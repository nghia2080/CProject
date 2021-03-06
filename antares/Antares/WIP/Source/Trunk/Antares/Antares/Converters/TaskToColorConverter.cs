﻿using Repository.Sync;
using System;
using Windows.UI.Xaml.Data;
using Repository.MODELs;
namespace Antares.Converters
{
    public class TaskToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int cateID = 0;
            if (value is TaskModel)
            {
                var model = value as TaskModel;
                cateID = model.Category;

            }
            else if (value is GroupCollection)
            {
                var model = value as GroupCollection;
                cateID = model.Id;
            }

            switch (cateID)
            {
                case 10:
                    return "#3c1e22";
                case 11:
                    return "#211e3c";
                case 12:
                    return "#1e3c3c";
                case 13:
                    return "#203c1e";
                case 14:
                    return "#3c231e";
                default:
                    return "#211e3c";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
