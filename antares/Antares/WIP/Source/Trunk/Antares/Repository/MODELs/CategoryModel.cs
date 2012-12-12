using AntaresShell.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MODELs
{
    public class CategoryModel  :BindableBase
    {
        private int _id;
        public int ID { get { return _id; } set { SetProperty(ref _id, value); } }

        private string _name;
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        private int _type;
        public int Type { get { return _type; } set { SetProperty(ref _type, value); } }

        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }
    }
}
