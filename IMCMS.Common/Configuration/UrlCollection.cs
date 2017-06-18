using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMCMS.Common.Configuration
{
    public class UrlCollection : ConfigurationElementCollection
    {
       public override ConfigurationElementCollectionType CollectionType {
          get { return
             ConfigurationElementCollectionType.AddRemoveClearMap; }
       }
 
       public UrlEntry this[int index] {
           get { return (UrlEntry)base.BaseGet(index); }
       }

       public new UrlEntry this[string name]
       {
           get { return (UrlEntry)base.BaseGet(name); }
       }
 
       protected override ConfigurationElement CreateNewElement() {
           return new UrlEntry();
       }
 
       protected override object GetElementKey(
             ConfigurationElement element) {
                 return ((UrlEntry)element).Regex;
       }

       public int IndexOf(UrlEntry action)
       {
          return BaseIndexOf(action);
       }

       public void Add(UrlEntry action)
       {
          BaseAdd(action);
       }

       public void Remove(UrlEntry action)
       {
          if (BaseIndexOf(action) > 0) {
              BaseRemove(action.Regex);
          }
       }
 
       public void RemoveAt(int index) {
          BaseRemoveAt(index);
       }
 
       public void Remove(string name) {
          BaseRemove(name);
       }
 
       public void Clear() {
          BaseClear();
       }
    }
}
