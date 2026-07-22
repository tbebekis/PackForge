// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Displays a modal busy message while a UI-owned operation is running.
/// </summary>
public partial class BusyDialog : Window
{
    // ● private fields
    bool fCanClose;

    // ● protected
    /// <summary>
    /// Prevents user-initiated closing while the operation is running.
    /// </summary>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!fCanClose)
            e.Cancel = true;

        base.OnClosing(e);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="BusyDialog"/> class.
    /// </summary>
    public BusyDialog()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BusyDialog"/> class.
    /// </summary>
    public BusyDialog(string Message)
        : this()
    {
        lblMessage.Text = Message;
    }

    // ● public
    /// <summary>
    /// Closes the dialog from code.
    /// </summary>
    public void CloseDialog()
    {
        fCanClose = true;
        Close();
    }

    // ● properties
    /// <summary>
    /// Gets or sets the displayed message.
    /// </summary>
    public string Message
    {
        get => lblMessage.Text;
        set => lblMessage.Text = value;
    }
}
