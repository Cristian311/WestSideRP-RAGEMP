using System;

namespace ServerSide
{
    public class PhoneMessage
    {
        public int ReceiverNumber { get; }
        public int SenderNumber { get; }
        public string Message { get; }
        DateTime SentTime { get; }
        public PhoneMessage(int receiverNumber, int senderNumber, string message)
        {
            ReceiverNumber = receiverNumber;
            SenderNumber = senderNumber;
            Message = message;
            SentTime = DateTime.Now;
        }
    }
}
