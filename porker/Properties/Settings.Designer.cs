﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace porker.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("zhongbao123")]
        public string PK_DEFAULT_PASS {
            get {
                return ((string)(this["PK_DEFAULT_PASS"]));
            }
            set {
                this["PK_DEFAULT_PASS"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15884156797")]
        public string PK_DEFAULT_USER {
            get {
                return ((string)(this["PK_DEFAULT_USER"]));
            }
            set {
                this["PK_DEFAULT_USER"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int PK_PLAY_REQ {
            get {
                return ((int)(this["PK_PLAY_REQ"]));
            }
            set {
                this["PK_PLAY_REQ"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://54.223.123.174:8083/poker/auth/login")]
        public string PK_LOGIN_AUTH_URL {
            get {
                return ((string)(this["PK_LOGIN_AUTH_URL"]));
            }
            set {
                this["PK_LOGIN_AUTH_URL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2000")]
        public int PK_WEB_TIMEOUT {
            get {
                return ((int)(this["PK_WEB_TIMEOUT"]));
            }
            set {
                this["PK_WEB_TIMEOUT"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("time.pool.aliyun.com")]
        public string PK_NTP_SERVER {
            get {
                return ((string)(this["PK_NTP_SERVER"]));
            }
            set {
                this["PK_NTP_SERVER"] = value;
            }
        }
    }
}
