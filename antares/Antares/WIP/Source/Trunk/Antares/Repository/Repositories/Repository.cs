using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AntaresShell.BaseClasses;
using Repository.MODELs;
using System.Linq;
using Repository.Repositories;
using Repository.Sync;

namespace Repository
{
    public class Repository
    {
        private Repository()
        {
        }

        public static Repository Instance
        {
            get { return Nested._instance; }
        }

        // Template vairable to store anything.
        public object Variable { get; set; }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly Repository _instance = new Repository();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }
        }

        async public Task<ObservableCollection<TaskModel>> GetAllTasksAsync()
        {
            // HARDCODED
            //var t1 = new TaskModel { Name = "Task 1", Description = "Depth first search", EndTime = 500, Period = 60, Priority = 0, ProjectID = 0, RepeatType = 0, StartTime = 400, Status = 1, ID = 0, UserID = 0};
            //var t2 = new TaskModel { Name = "Task 2", Description = "Dijkstra", EndTime = 600, Period = 60, Priority = 1, ProjectID = 0, RepeatType = 1, StartTime = 500, Status = 0, ID = 1, UserID = 1 };
            //var t3 = new TaskModel { Name = "Task 3", Description = "Floyd Warshall", EndTime = 700, Period = 60, Priority = 2, ProjectID = 1, RepeatType = 0, StartTime = 600, Status = 0, ID = 2, UserID = 1 };
            //var t4 = new TaskModel { Name = "Task 4", Description = "Bellman Ford", EndTime = 800, Period = 60, Priority = 0, ProjectID = 1, RepeatType = 0, StartTime = 700, Status = 1, ID = 3, UserID = 1 };
            //var t5 = new TaskModel { Name = "Task 5", Description = "Breath first search", EndTime = 900, Period = 60, Priority = 1, ProjectID = 1, RepeatType = 0, StartTime = 800, Status = 1, ID = 4, UserID = 1 };
            //var t6 = new TaskModel { Name = "Task 6", Description = "Dynamic programming" };
            //var t7 = new TaskModel { Name = "Task 7", Description = "Aho Corasick" };
            //var t8 = new TaskModel { Name = "Task 8", Description = "Sieve of Eratosthenes" };
            //var t9 = new TaskModel { Name = "Task 9", Description = "Rabin Karp" };
            //var t10 = new TaskModel { Name = "Task 10", Description = "Knutt Morris Pratt" };
            //var t11 = new TaskModel { Name = "Task 11", Description = "Backtracking" };
            //var t12 = new TaskModel { Name = "Task 12", Description = "Recursion" };
            //var t13 = new TaskModel { Name = "Task 13", Description = "Gaussian elimination" };
            //var t14 = new TaskModel { Name = "Task 14", Description = "Quick sort" };
            //var t15 = new TaskModel { Name = "Task 15", Description = "Block sort" };
            //var t16 = new TaskModel { Name = "Task 16", Description = "Radix sort" };
            //var t17 = new TaskModel { Name = "Task 17", Description = "Prefix tree - Trie" };
            //var t18 = new TaskModel { Name = "Task 18", Description = "Suffix tree - Suffix array" };
            //var t19 = new TaskModel { Name = "Task 19", Description = "Interval tree - Segment tree" };
            //var t20 = new TaskModel { Name = "Task 20", Description = "Splay tree" };
            //var t21 = new TaskModel { Name = "Task 21", Description = "Ternary Search" };
            //var t22 = new TaskModel { Name = "Task 22", Description = "Binary Search" };
            //var t23 = new TaskModel { Name = "Task 23", Description = "Tarjan" };
            //var t24 = new TaskModel { Name = "Task 24", Description = "Binary search tree" };

            //return new ObservableCollection<TaskModel> { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24 };
            return new ObservableCollection<TaskModel>(await TaskRepository.Instance.GetAllTasksForUser(GlobalData.MyUserID));
        } 

        async public Task<SearchableBaseModel[]> GetSeachableBaseModelAsync()
        {
             return (await GetAllTasksAsync()).ToArray();
        } 
    }
}
