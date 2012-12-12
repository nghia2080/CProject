using System.Collections.Generic;
using AntaresShell.NavigatorProvider;
using AntaresShell.Utilities;
using Repository.LiveConnection;
using Repository.MODELs;

namespace Repository.Sync
{
    public class GlobalData
    {
        public static int MyUserID { get; set; }

        public static int SelectedWeekIndex { get; set; }
        public static int SelectedMonthIndex { get; set; }
        public static int NumberOfRows { get; set; }

        private static int _selectedProjects = -1;

        public static int SelectedProjects
        {
            get { return _selectedProjects; }
            set
            {
                _selectedProjects = value;

                //if (_selectedProjects != null)
                //{
                //    if (_selectedProjects.Count >= 1)
                //    {
                //        Navigator.Instance.BottomAppBar.IsSticky = true;

                //        if (!AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.DelProjectBtn))
                //        {
                //            AppButtonManager.Instance.GridAppbar.Children.Insert(0, AppButtonManager.Instance.DelProjectBtn);
                //        }
                //    }
                //    else
                //    {
                //        Navigator.Instance.BottomAppBar.IsSticky = false;

                //        if (AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.DelProjectBtn))
                //        {
                //            AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.DelProjectBtn);
                //        }
                //    }
                //}
                //else
                //{
                //    Navigator.Instance.BottomAppBar.IsSticky = false;

                //    if (AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.DelProjectBtn))
                //    {
                //        AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.DelProjectBtn);
                //    }
                //}
            }
        }

        private static int _numberSelectionOfTasks;

        public static int NumberSelectionOfTasks
        {
            get { return _numberSelectionOfTasks; }
            set
            {
                _numberSelectionOfTasks = value;

                if (_numberSelectionOfTasks >= 1)
                {
                    Navigator.Instance.BottomAppBar.IsSticky = true;

                    //if (!AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.DelProjectBtn))
                    //{
                    //    AppButtonManager.Instance.GridAppbar.Children.Insert(0, AppButtonManager.Instance.DelProjectBtn);
                    //}

                    //    if (AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.ViewProjectBtn))
                    //    {
                    //        AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.ViewProjectBtn);
                    //    }

                    //}
                    //else if (_numberSelectionOfTasks == 1)
                    //{
                    //    Navigator.Instance.BottomAppBar.IsSticky = true;
                    //    if (!AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.DelProjectBtn))
                    //    {
                    //        AppButtonManager.Instance.GridAppbar.Children.Insert(0, AppButtonManager.Instance.DelProjectBtn);
                    //    }

                    //    if (!AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.ViewProjectBtn))
                    //    {
                    //        AppButtonManager.Instance.GridAppbar.Children.Insert(0, AppButtonManager.Instance.ViewProjectBtn);
                    //    }

                }
                else
                {
                    Navigator.Instance.BottomAppBar.IsSticky = false;

                    //if (AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.DelProjectBtn))
                    //{
                    //    AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.DelProjectBtn);
                    //}

                    //if (AppButtonManager.Instance.GridAppbar.Children.Contains(AppButtonManager.Instance.ViewProjectBtn))
                    //{
                    //    AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.ViewProjectBtn);
                    //}
                }
            }
        }

        public static LiveUserInformationModel UserInformationModel { get; set; }

        public static TaskModel TemporaryTask { get; set; }
    }
}
