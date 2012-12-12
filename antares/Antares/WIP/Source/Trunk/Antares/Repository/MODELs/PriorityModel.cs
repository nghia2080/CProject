using AntaresShell.BaseClasses;

namespace Repository.MODELs
{
    public class PriorityModel : BindableBase
    {
        private int _id;
        public int ID { get { return _id; } set { SetProperty(ref _id, value); } }

        private string _name;
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }
    }
}
