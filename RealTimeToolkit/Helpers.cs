namespace RealTimeToolkit
{
    using Common;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Помощни методи за работа с пакетите
    /// </summary>
    public static class Helpers
    {
        /// <summary>Извличана на WebSocket пакет</summary>
        /// <param name="data">Пакета за трансформиране</param>
        /// <returns>Информация за пакета</returns>
        public static FrameMaskData GetFrameData(byte[] data)
        {
            // Get the opcode of the frame
            int opcode = data[0] - 128;

            // If the length of the message is in the 2 first indexes
            if (data[1] - 128 <= 125)
            {
                int dataLength = (data[1] - 128);
                return new FrameMaskData(dataLength, 2, dataLength + 6, (OpcodeType)opcode);
            }

            // If the length of the message is in the following two indexes
            if (data[1] - 128 == 126)
            {
                // Combine the bytes to get the length
                int dataLength = BitConverter.ToInt16(new byte[] { data[3], data[2] }, 0);
                return new FrameMaskData(dataLength, 4, dataLength + 8, (OpcodeType)opcode);
            }

            // If the data length is in the following 8 indexes
            if (data[1] - 128 == 127)
            {
                // Get the following 8 bytes to combine to get the data 
                byte[] combine = new byte[8];
                for (int i = 0; i < 8; i++) combine[i] = data[i + 2];

                // Combine the bytes to get the length
                //int dataLength = (int)BitConverter.ToInt64(new byte[] { Data[9], Data[8], Data[7], Data[6], Data[5], Data[4], Data[3], Data[2] }, 0);
                int dataLength = (int)BitConverter.ToInt64(combine, 0);
                return new FrameMaskData(dataLength, 10, dataLength + 14, (OpcodeType)opcode);
            }

            // error
            return new FrameMaskData(0, 0, 0, 0);
        }

        /// <summary>Извлича кода на пакета</summary>
        /// <param name="frame">Пакета от които ще се извлече код</param>
        /// <returns>Кода на пакета</returns>
        public static OpcodeType GetFrameOpcode(byte[] frame)
        {
            return (OpcodeType)frame[0] - 128;
        }

        /// <summary>Извлича декодирано съобщение</summary>
        /// <param name="Data">пакет за декодиране</param>
        /// <returns>Декодиран пакет</returns>
        public static string GetDataFromFrame(byte[] Data)
        {
            FrameMaskData frameData = GetFrameData(Data);
            
            byte[] decodeKey = new byte[4];
            for (int i = 0; i < 4; i++) decodeKey[i] = Data[frameData.KeyIndex + i];

            int dataIndex = frameData.KeyIndex + 4;
            int count = 0;
            
            for (int i = dataIndex; i < frameData.TotalLenght && Data.Length > i; i++)
            {
                Data[i] = (byte)(Data[i] ^ decodeKey[count % 4]);
                count++;
            }
            
            return Encoding.Default.GetString(Data, dataIndex, frameData.DataLength);
        }

        /// <summary>Проверка за валидноста на масив</summary>
        /// <param name="buffer">Масива за вслидиране</param>
        /// <returns>статуса на валидацията</returns>
        public static bool GetIsBufferValid(ref byte[] buffer)
        {
            if (buffer == null) return false;
            if (buffer.Length <= 0) return false;

            return true;
        }

        /// <summary>Трансформиране на съобщене в пакет за предаване</summary>
        /// <param name="message">Съобщениоето което ще се трансформира</param>
        /// <param name="opcode">Кода на пакета</param>
        /// <returns>Резултатния пакет за изпращане</returns>
        public static byte[] GetFrameFromString(string message, OpcodeType opcode = OpcodeType.Text)
        {
            byte[] response;
            byte[] bytesRaw = Encoding.Default.GetBytes(message);
            byte[] frame = new byte[10];

            int indexStartRawData = -1;
            int length = bytesRaw.Length;

            frame[0] = (byte)(128 + (int)opcode);
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new byte[indexStartRawData + length];

            int i, reponseIdx = 0;

            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

        /// <summary>Извличане на ключ за одговор</summary>
        /// <param name="Key">Ключ на одговора</param>
        /// <returns></returns>
        public static string HashKey(string Key)
        {
            const string handshakeKey = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            string longKey = Key + handshakeKey;

            SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(longKey));

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>Извличане на http одговор за клинт</summary>
        /// <param name="кey">Ключ на одговора</param>
        /// <returns></returns>
        public static string GetHandshakeResponse(string кey)
        {
            return string.Format("HTTP/1.1 101 Switching Protocols\nUpgrade: WebSocket\nConnection: Upgrade\nSec-WebSocket-Accept: {0}\r\n\r\n", кey);
        }

        /// <summary>Извличане на ключа от заявката за свързване</summary>
        /// <param name="httpRequest">Http заявката</param>
        /// <returns></returns>
        public static string GetHandshakeRequestKey(string httpRequest)
        {
            int keyStart = httpRequest.IndexOf("Sec-WebSocket-Key: ") + 19;
            string key = null;

            for (int i = keyStart; i < (keyStart + 24); i++)
            {
                key += httpRequest[i];
            }
            return key;
        }

        /// <summary>Създаване на уникален идентификатор</summary>
        /// <param name="prefix">Префикс на идентификатора</param>
        /// <param name="Length">Дължина на идентификатора</param>
        /// <returns>Резултата от съзгаването на идентификатор</returns>
        public static string CreateGuid(string prefix, int length = 16)
        {
            string final = null;
            string ids = "0123456789abcdefghijklmnopqrstuvwxyz";

            Random random = new Random();

            for (short i = 0; i < length; i++)
                final += ids[random.Next(0, ids.Length)];

            if (prefix == null) return final;

            return string.Format("{0}-{1}", prefix, final);
        }
    }
}
