using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T m_Instance;

	/// <summary>
	/// 单例
	/// </summary>
	public static T _Instance => m_Instance;

	protected virtual void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = (T)this;
		}
		else
		{
			Destroy(gameObject);
			InstanceInitFail();
			return;
		}
	}

	/// <summary>
	/// 单例初始化失败
	/// </summary>
	protected virtual void InstanceInitFail() { }

}
