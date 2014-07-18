using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

public class MyDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>
{


    private System.Collections.Generic.List<TKey> mcolKeys = new List<TKey>();
    /// <summary>
    /// Gets an item by its index
    /// </summary>
    /// <param name="Index"></param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public TValue this[int Index]
    {
        get { return base[mcolKeys[Index]]; }
    }

    /// <summary> 
    /// Adds the specified key and value to the dictionary 
    /// </summary> 
    /// <param name="key"></param> 
    /// <param name="value"></param> 
    public new void Add(TKey key, TValue value)
    {
        mcolKeys.Add(key);
        base.Add(key, value);

    }

    /// <summary>
    /// Remove an item by its index
    /// </summary>
    /// <param name="Index"></param>
    /// <remarks></remarks>
    public void RemoveAt(int Index)
    {
        base.Remove(mcolKeys[Index]);
        mcolKeys.RemoveAt(Index);
    }

    /// <summary> 
    /// Find by name 
    /// </summary> 
    /// <param name="Name"></param> 
    /// <returns></returns> 
    public TValue Find(TKey Name)
    {
        if (this.ContainsKey(Name))
        {
            return this[Name];
        }
        return default(TValue);
    }

    /// <summary>
    /// Return a get from the specified index
    /// </summary>
    /// <param name="Index"></param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public TKey Key(int Index)
    {
        return mcolKeys[Index];
    }

    /// <summary>
    /// Clears the collection
    /// </summary>
    /// <remarks></remarks>
    public new void Clear()
    {
        mcolKeys.Clear();
        base.Clear();
    }

}