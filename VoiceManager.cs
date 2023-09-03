using System;
using System.Collections;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime;
using UnityEngine;
using VivoxUnity;
using Unity.Services.Core;
using Unity.Services.Vivox;
using Unity.Services.Authentication;
using Mirror;

#if AUTH_PACKAGE_PRESENT
using Unity.Services.Authentication;
#endif

/// <summary>
/// 语音管理器
/// </summary>
public class VoiceManager : Singleton<VoiceManager>
{
    public bool _IsSeriver;

    [SerializeField]
    private string _key;
    [SerializeField]
    private string _issuer;
    [SerializeField]
    private string _domain;
    [SerializeField]
    private string _server;

    private bool m_IsInit;

    public ILoginSession LoginSession;

    private Account m_Account;
    private Client _client => VivoxService.Instance.Client;

    public GameObject vivoxui;
    public GameObject vivoxuierror;


    protected override void Awake()
    {
        base.Awake();
        if (_IsSeriver)
            return;
        InitVoiceManager();
        DontDestroyOnLoad(this);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Login(SystemInfo.deviceName);
            if (!_IsSeriver)
             StartCoroutine(Uierror());
        }
    }

    /// <summary>
    /// 初始化语音管理器
    /// </summary>
    public async void InitVoiceManager()
    {
        if (string.IsNullOrEmpty(_key) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_domain) || string.IsNullOrEmpty(_server))
        {
            Debug.Log("语音管理器:有至少一个参数未设置!");
            return;
        }
        await UnityServices.InitializeAsync(new InitializationOptions().SetVivoxCredentials(_server, _domain, _issuer, _key));
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        VivoxService.Instance.Initialize();
        m_IsInit = true;
    }


    public void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText, bool transmissionSwitch = true, Channel3DProperties properties = null)
    {
        if (LoginSession.State == LoginState.LoggedIn)
        {
            Channel channel = new Channel(channelName, channelType, properties);

            IChannelSession channelSession = LoginSession.GetChannelSession(channel);

            channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch, channelSession.GetConnectToken(), ar =>
            {
                try
                {
                    channelSession.EndConnect(ar);
                }
                catch (Exception e)
                {
                   
                   
                    Debug.LogError($"Could not connect to channel: {e.Message}");
                    return;
                }
            });
        }
        else
        {
            
            Debug.LogError("Can't join a channel when not logged in.");
        }
    }
    IEnumerator Uierror()
    {
        vivoxuierror.SetActive(true);
        yield return new WaitForSeconds(2);
        vivoxuierror.SetActive(false);
    }
    public void Login(string displayName = null)
    {
        var account = new Account(displayName);

        LoginSession = VivoxService.Instance.Client.GetLoginSession(account);
        LoginSession.PropertyChanged += LoginSession_PropertyChanged;

        LoginSession.BeginLogin(LoginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
        {
            try
            {
                LoginSession.EndLogin(ar);
            }
            catch
            {
                // 取消绑定您订阅的所有登录会话相关事件。
                // 处理错误
                return;
            }

            // 至此，我们已经成功请求登录。 
            // 当您能够加入频道时，LoginSession.State 将设置为 LoginState.LoggedIn。
            // 参考 LoginSession_PropertyChanged()
        });
    }

    // 在本例中，我们在 LoginState 更改为 LoginState.LoggedIn 后立即加入频道。
    // 在实际游戏中，何时加入频道会因实现而异。
    private void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Debug.Log("加入成功");
        vivoxui.SetActive(true);
        var loginSession = (ILoginSession)sender;
        if (e.PropertyName == "State")
        {
            if (loginSession.State == LoginState.LoggedIn)
            {
                bool connectAudio = true;
                bool connectText = true;

                // 这会将您带入回声频道，您可以在其中听到自己的讲话。
                // 如果能听到自己的声音，那么一切正常，您已准备好可将 Vivox 集成到项目中。
                //JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                // 要测试多个用户，请尝试加入非位置频道。
                JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
            }
        }
    }

}
