using System.Runtime.Serialization;

namespace Repository.LiveConnection
{
    [KnownType(typeof(LiveUserInformationModel))]
    [DataContract]
    public class LiveUserInformationModel
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Link { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Gender { get; set; }

        [DataMember]
        public string Locale { get; set; }

        [DataMember]
        public string UpdatedTime { get; set; }
    }
}
