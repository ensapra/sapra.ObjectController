/* using System;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController{
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();
	
	[SerializeReference]
	private List<TValue> values = new List<TValue>();
	
	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach(KeyValuePair<TKey, TValue> pair in this)
		{
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}
	
	// load dictionary from lists
	public void OnAfterDeserialize()
	{
        Debug.Log(this.Keys.Count); 
		//this.Clear();

		for(int i = 0; i < this.keys.Count; i++){
            Debug.Log(values[i]);
			this.Add(this.keys[i], this.values[i]);
        }
    }
}
[Serializable]
public class DictioanryTypeRoutine{
    [SerializeReference] List<Type> keys = new List<Type>();
    [SerializeReference] List<Routine> values = new List<Routine>();
    void Add(Type t, Routine r){
        if(!keys.Contains(t)){
            keys.Add(t);
            values.Add(r);
        }
    }

    void Remove(Type t, Routine r){
        if(!keys.Contains(t))
        {
            values.Add(r);
            keys.Remove(t);
        }
    }
}

}
 */