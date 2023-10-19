namespace RealTimeToolkit.Common
{
    /// <summary>
    /// Информация за пакета на комуникация
    /// </summary>
    public struct FrameMaskData
    {
        public int DataLength, KeyIndex, TotalLenght;
        public OpcodeType Opcode;

        public FrameMaskData(int DataLength, int KeyIndex, int TotalLenght, OpcodeType Opcode)
        {
            this.DataLength = DataLength;
            this.KeyIndex = KeyIndex;
            this.TotalLenght = TotalLenght;
            this.Opcode = Opcode;
        }
    }
}
