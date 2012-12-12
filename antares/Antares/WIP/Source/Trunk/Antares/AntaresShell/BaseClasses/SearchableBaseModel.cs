using System.Runtime.Serialization;
using System.Text;

namespace AntaresShell.BaseClasses
{
    /// <summary>
    /// A class to be used as the base for all searchable objects in VCM.
    /// </summary>
    [DataContract]
    public class SearchableBaseModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the SearchableBaseModel class.
        /// </summary>
        public SearchableBaseModel()
        {
            Name = string.Empty;
            Description = string.Empty;
            // NavigateInfo = new List<object> { NavigateType.Dashboard, ViewModelName.MESSAGES_VM_CODE };
        }

        private string _name;

        /// <summary>
        /// Gets or sets the item's name.
        /// </summary>
        /// <value>A string represents the item's name.</value>
        [DataMember]
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        private string _description;

        /// <summary>
        /// Gets or sets the item's description.
        /// </summary>
        /// <value>A string represents the item's description.</value>
        [DataMember]
        public string Description { get { return _description; } set { SetProperty(ref _description, value); } }

        /// <summary>
        /// Collects every searchable property of the object and appends it to a searchable string.
        /// </summary>
        /// <returns>A string used in search progress.</returns>
        public virtual string ToSearchableString()
        {
            return new StringBuilder(Name ?? string.Empty).Append("|")
                                           .Append(Description ?? string.Empty).ToString();
        }
    }
}