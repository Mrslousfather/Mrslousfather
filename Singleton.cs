using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���͵�����
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T m_Instance;

	/// <summary>
	/// ����
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
	/// ������ʼ��ʧ��
	/// </summary>
	protected virtual void InstanceInitFail() { }

}
