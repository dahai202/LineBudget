using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X2Lib.Common.Algorithm.NestedTree
{
    /// <summary>
    /// build the nested Tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NestedTree
    {
       public NTNode treeRoot;

       public void InsertNode(object newObj, int leftIndex, int rightIndex)
       {
           if (treeRoot != null)
           {
               if (true)
               {
                   treeRoot.InsertNode(newObj,leftIndex,rightIndex);
               }
           }
           else
           {
               treeRoot = new NTNode();
               treeRoot.leftIndex = leftIndex;
               treeRoot.rightIndex = rightIndex;
               treeRoot.nodeObj = newObj;  ////temperal root
           }
       }
    }
}
