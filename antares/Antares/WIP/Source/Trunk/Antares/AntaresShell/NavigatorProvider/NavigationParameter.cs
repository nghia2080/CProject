using System.Runtime.Serialization;

namespace AntaresShell.NavigatorProvider
{
    [DataContract]
    public class NavigationParameter123
    {
        [DataMember]
        public ProjectManagerEnum? SubPage { get; set; }
        [DataMember]
        public object Parameter { get; set; }
    }

    public enum ProjectManagerEnum
    {
        Overview = 1,
        Detail = 2,
        Task = 3
    }
}
