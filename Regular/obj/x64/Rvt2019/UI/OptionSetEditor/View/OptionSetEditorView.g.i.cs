﻿#pragma checksum "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7CEAFA176E2377A11879EC8CEB40FFE913F6F4EED5A76830E596A95FA28A0E01"
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


namespace Regular.UI.OptionSetEditor.View {
    
    
    /// <summary>
    /// OptionSetEditorView
    /// </summary>
    public partial class OptionSetEditorView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 31 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DataGridOptions;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonCancel;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonOk;
        
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
            System.Uri resourceLocater = new System.Uri("/Regular;component/ui/optionseteditor/view/optionseteditorview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
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
            
            #line 15 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
            ((Regular.UI.OptionSetEditor.View.OptionSetEditorView)(target)).Loaded += new System.Windows.RoutedEventHandler(this.OptionSetEditorView_OnLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DataGridOptions = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 3:
            this.ButtonCancel = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
            this.ButtonCancel.Click += new System.Windows.RoutedEventHandler(this.ButtonCancel_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ButtonOk = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\..\..\..\..\UI\OptionSetEditor\View\OptionSetEditorView.xaml"
            this.ButtonOk.Click += new System.Windows.RoutedEventHandler(this.ButtonOk_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

