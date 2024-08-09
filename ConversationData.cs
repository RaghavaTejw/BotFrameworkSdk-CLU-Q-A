using System.Collections.Generic;

namespace EmptyBot1
{
    public class ConversationData
    {
        public string utterance { get; set; }
        public string intent { get; set; }
        public List<string> answers { get; set; }=new List<string>();
    }
}
