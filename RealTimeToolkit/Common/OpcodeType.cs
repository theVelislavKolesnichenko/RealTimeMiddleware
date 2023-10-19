namespace RealTimeToolkit.Common
{
    /// <summary>
    /// Enum for opcode types
    /// </summary>
    public enum OpcodeType
    {
        /* Denotes a continuation code */
        Fragment = 0,

        /* Denotes a text code */
        Text = 1,

        /* Denotes a binary code */
        Binary = 2,

        /* Denotes a closed connection */
        ClosedConnection = 8,

        /* Denotes a ping*/
        Ping = 9,

        /* Denotes a pong */
        Pong = 10
    }
}
