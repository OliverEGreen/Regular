﻿#pragma checksum "..\..\..\..\Views\RuleManagerView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "012366E3770F754E7E648130D6A8B8B0369D8EC8D3FE2BE0071939D75922ADBA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Regular.Views {
    
    
    /// <summary>
    /// RuleManagerView
    /// </summary>
    public partial class RuleManagerView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Regular.Views.RuleManagerView RuleManagerWindow;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonMoveRulePartUp;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonMoveRulePartDown;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonDuplicateRule;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonStopStartRule;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonAddNewRule;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer RulesScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListBoxRegexRules;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\Views\RuleManagerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonClose;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Regular;component/views/rulemanagerview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\RuleManagerView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.RuleManagerWindow = ((Regular.Views.RuleManagerView)(target));
            return;
            case 2:
            this.ButtonMoveRulePartUp = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.ButtonMoveRulePartDown = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.ButtonDuplicateRule = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.ButtonStopStartRule = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.ButtonAddNewRule = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.RulesScrollViewer = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 87 "..\..\..\..\Views\RuleManagerView.xaml"
            this.RulesScrollViewer.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.RegexRulesScrollViewer_PreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ListBoxRegexRules = ((System.Windows.Controls.ListBox)(target));
            return;
            case 9:
            this.ButtonClose = ((System.Windows.Controls.Button)(target));
            
            #line 160 "..\..\..\..\Views\RuleManagerView.xaml"
            this.ButtonClose.Click += new System.Windows.RoutedEventHandler(this.ButtonClose_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

