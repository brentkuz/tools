//Author: Brent Kuzmanich
//Comment: Class used to map a SharePoint list to another.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SPListCopyConsole
{
    class ListCopier
    {        
        private ClientContext sourceContext;
        private ClientContext targetContext;
        private List sourceList;
        private List targetList;
        private Dictionary<string, string> columnMaps;
        private Dictionary<string, Delegate> columnFormats; 

        //ctor
        public ListCopier(string sourcePath, string sourceName, string targetPath, string targetName)
        {
            this.sourceContext = new ClientContext(sourcePath);
            this.targetContext = new ClientContext(targetPath);
            this.sourceList = this.sourceContext.Web.Lists.GetByTitle(sourceName);
            this.targetList = this.targetContext.Web.Lists.GetByTitle(targetName);
            this.columnMaps = new Dictionary<string, string>();
            this.columnFormats = new Dictionary<string, Delegate>();
        }

        #region public methods
        //Method to create mappings between list columns
        public void AddColumnMaps(string col)
        {
            //split individual maps
            string[] parts = col.Split(',');
            foreach(string part in parts)
            {
                //split on '->'
                var tmp  = part.Split(new string[]{"->"}, StringSplitOptions.None);
                string sCol = tmp[0],
                       tCol = tmp[1];
                columnMaps.Add(tCol, sCol);                
            }
        }
        //Method to add a format to the resultant colum data
        public void AddColumnFormat(string targColName, Delegate del)
        {
            if(columnMaps[targColName] != null)
            {
                columnFormats[targColName] = del;
            }
        }
        //Method to process copying
        public void CopyListItems()
        {
            if(columnMaps.Count > 0)
            {
                CamlQuery query = CamlQuery.CreateAllItemsQuery();
                ListItemCollection items = sourceList.GetItems(query);
                sourceContext.Load(items);
                sourceContext.ExecuteQuery();

                //iterate and copy
                foreach(ListItem item in items)
                {
                    ListItemCreationInformation createInfo = new ListItemCreationInformation();
                    ListItem newItem = targetList.AddItem(createInfo);
                    //map cols
                    foreach(string col in columnMaps.Keys)
                    {
                        string txt = item[columnMaps[col]].ToString();
                        //format col contents in target list
                        if (columnFormats.ContainsKey(col))
                        {
                            var frmt = columnFormats[col];
                            if (frmt != null)
                            {
                                //apply formatting
                                txt = frmt.DynamicInvoke(txt).ToString();
                            }
                        }
                        newItem[col] = txt;
                    }
                    
                    newItem.Update();                    
                    targetContext.ExecuteQuery();
                    //notify copied item
                    Console.WriteLine("Item: " + item.Id + " copied");
                }
            }
        }
        #endregion
    }
}
