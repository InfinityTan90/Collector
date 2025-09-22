using Collection.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Model
{
    static class Data
    {
        static public ObservableCollection<Items> AuList = new ObservableCollection<Items>();
        static public ObservableCollection<Items> AuList_Filtered = new ObservableCollection<Items>();

        static public string Directory;
    }
}