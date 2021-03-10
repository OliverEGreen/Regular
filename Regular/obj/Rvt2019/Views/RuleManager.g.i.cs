﻿#pragma checksum "..\..\..\Views\RuleManager.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9C1F573512D1D3668ACE75CCA14C4DFC085E26C6AB1DE108D6B29773A82CA7F5"
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
    /// RuleManager
    /// </summary>
    public partial class RuleManager : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 30 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonMoveRulePartUp;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonMoveRulePartDown;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonDuplicateRule;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonStopStartRule;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonAddNewRule;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer RulesScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\Views\RuleManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListBoxRegexRules;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\Views\RuleManager.xaml"
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
            System.Uri resourceLocater = new System.Uri("/Regular;component/views/rulemanager.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\RuleManager.xaml"
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
            this.ButtonMoveRulePartUp = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\..\Views\RuleManager.xaml"
            this.ButtonMoveRulePartUp.Click += new System.Windows.RoutedEventHandler(this.ReorderUpButton_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ButtonMoveRulePartDown = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\Views\RuleManager.xaml"
            this.ButtonMoveRulePartDown.Click += new System.Windows.RoutedEventHandler(this.ReorderDownButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ButtonDuplicateRule = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\Views\RuleManager.xaml"
            this.ButtonDuplicateRule.Click += new System.Windows.RoutedEventHandler(this.ButtonDuplicateRule_OnClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ButtonStopStartRule = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\..\Views\RuleManager.xaml"
            this.ButtonStopStartRule.Click += new System.Windows.RoutedEventHandler(this.ButtonStopStartRule_OnClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ButtonAddNewRule = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\..\Views\RuleManager.xaml"
            this.ButtonAddNewRule.Click += new System.Windows.RoutedEventHandler(this.ButtonAddNewRule_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.RulesScrollViewer = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 90 "..\..\..\Views\RuleManager.xaml"
            this.RulesScrollViewer.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.RegexRulesScrollViewer_PreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ListBoxRegexRules = ((System.Windows.Controls.ListBox)(target));
            
            #line 97 "..\..\..\Views\RuleManager.xaml"
            this.ListBoxRegexRules.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.ListBoxRegexRules_OnMouseDown);
            
            #line default
            #line hidden
            
            #line 98 "..\..\..\Views\RuleManager.xaml"
            this.ListBoxRegexRules.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListBoxRegexRules_OnSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ButtonClose = ((System.Windows.Controls.Button)(target));
            
            #line 165 "..\..\..\Views\RuleManager.xaml"
            this.ButtonClose.Click += new System.Windows.RoutedEventHandler(this.ButtonClose_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 8:
            
            #line 137 "..\..\..\Views\RuleManager.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditRegexRuleButton_Click);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 148 "..\..\..\Views\RuleManager.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteRegexRuleButton_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

