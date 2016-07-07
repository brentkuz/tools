/*
 * Author: Brent Kuzmanich
 * Comment: Static helper methods used to get a generic list of entities
 * from a data reader via reflection. * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class EntityReaderHelper
    {
        //Generic method to get a list<T> from SqlDataReader 
        public static IList GetListFromReader(SqlCommand cmd, Type boType)
        {
            object bo = Activator.CreateInstance(boType);
            var props = bo.GetType().GetProperties();
            IList list = CreateList(boType);

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            var value = dr[i];
                            var prop = bo.GetType().GetProperty(props[i].Name);
                            Type propType = prop.PropertyType;
                            if (propType == typeof(Guid))
                                value = new Guid(value.ToString());
                            else
                                value = (value.ToString() != "") ? Convert.ChangeType(value, propType) : "";
                            prop.SetValue(bo, value);
                        }
                        list.Add(bo);
                        bo = Activator.CreateInstance(boType);
                    }
                }
                return list;
            }
        }

        //Method to create a list from a given type
        private static IList CreateList(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(new[] { type });
            return (IList)Activator.CreateInstance(listType);
        }   
    }
}
