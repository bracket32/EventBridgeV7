using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Java.Lang;
using Android.Content;
using Android.Runtime;
using System;
using System.Collections.Generic;

namespace EventBridgeV7.Droid
{
    [Activity(Label = "EventBridgeV7", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, IFacebookCallback
    {
        private ICallbackManager mCallBackManager;
        private FbProfileTracker appProfileTracker;
        public void OnCancel()
        {
            //throw new System.NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            string temp = error.LocalizedMessage;
            //throw new System.NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;

            //persist the access token to call the api for the events
            //from here you want to get the profile with the token
            AccessToken token = loginResult.AccessToken;
        }

        protected override void OnDestroy()
        {
            appProfileTracker.StopTracking();
            base.OnDestroy();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //local button
            Button loginFacebook = FindViewById<Button>(Resource.Id.btnLoginFacebook);
            //check to see if we are logged in
            if (AccessToken.CurrentAccessToken != null && Profile.CurrentProfile != null)
            {
                loginFacebook.Text = "Log Out of Facebook...";
            }


            mCallBackManager = CallbackManagerFactory.Create();


            //localbutton
            LoginManager.Instance.RegisterCallback(mCallBackManager, this);
            loginFacebook.Click += (o, e) =>
            {
                //local button
                if (AccessToken.CurrentAccessToken != null && Profile.CurrentProfile != null)
                {
                    LoginManager.Instance.LogOut();
                    loginFacebook.Text = "Log In to Facebook...";
                }
                else
                {
                    LoginManager.Instance.LogInWithReadPermissions(this, new List<string>() { "basic_info" });
                    loginFacebook.Text = "Log Out of Facebook...";
                }
            };


            appProfileTracker = new FbProfileTracker();
            appProfileTracker.StartTracking();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }
    }

    //this is a listener for the profile
    public class FbProfileTracker : ProfileTracker
    {
        //create an event with the type of args we made specifically for this
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        //this gets called by FB sdk when the profile changes
        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            //any function that subscfribes to this event will fire...
            if (mOnProfileChanged != null)
            {
                mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(currentProfile));
            }
        }
    }
    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile mProfile;

        public OnProfileChangedEventArgs(Profile profile)
        {
            mProfile = profile;
        }
    }
}

