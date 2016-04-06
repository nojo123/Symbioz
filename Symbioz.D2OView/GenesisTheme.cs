

// Create a  c# project (winforms), add a new blank class, remove preset, use this code, then build the winform project. The custom controls will be added to the toolbox. Lots of options in the properties menu. Have fun!

// by M.A.

/*
Add a Reference to the Visual Basic .NET Run-Time Library
     
In a Visual C# application, click the Project menu, and then click Add Reference.
In the Component Name list, click Microsoft Visual Basic .NET Runtime to add Microsoft.VisualBasic.dll.
This should remove the strings error.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;

#region Themebase
//------------------
//Creator: aeonhack
//Site: elitevs.net
//Created: 08/02/2011
//Changed: 12/06/2011
//Version: 1.5.4
//------------------

abstract class ThemeContainer154 : ContainerControl
{

    #region " Initialization "

    protected Graphics G;

    protected Bitmap B;
    public ThemeContainer154()
    {
        SetStyle((ControlStyles)139270, true);

        _ImageSize = Size.Empty;
        Font = new Font("Verdana", 8);

        MeasureBitmap = new Bitmap(1, 1);
        MeasureGraphics = Graphics.FromImage(MeasureBitmap);

        DrawRadialPath = new GraphicsPath();

        InvalidateCustimization();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (DoneCreation)
            InitializeMessages();

        InvalidateCustimization();
        ColorHook();

        if (!(_LockWidth == 0))
            Width = _LockWidth;
        if (!(_LockHeight == 0))
            Height = _LockHeight;
        if (!_ControlMode)
            base.Dock = DockStyle.Fill;

        Transparent = _Transparent;
        if (_Transparent && _BackColor)
            BackColor = Color.Transparent;

        base.OnHandleCreated(e);
    }

    private bool DoneCreation;
    protected override sealed void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);

        if (Parent == null)
            return;
        _IsParentForm = Parent is Form;

        if (!_ControlMode)
        {
            InitializeMessages();

            if (_IsParentForm)
            {
                ParentForm.FormBorderStyle = _BorderStyle;
                ParentForm.TransparencyKey = _TransparencyKey;

                if (!DesignMode)
                {
                    ParentForm.Shown += FormShown;
                }
            }

            Parent.BackColor = BackColor;
        }

        OnCreation();
        DoneCreation = true;
        InvalidateTimer();
    }

    #endregion

    private void DoAnimation(bool i)
    {
        OnAnimation();
        if (i)
            Invalidate();
    }

    protected override sealed void OnPaint(PaintEventArgs e)
    {
        if (Width == 0 || Height == 0)
            return;

        if (_Transparent && _ControlMode)
        {
            PaintHook();
            e.Graphics.DrawImage(B, 0, 0);
        }
        else
        {
            G = e.Graphics;
            PaintHook();
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        ThemeShare.RemoveAnimationCallback(DoAnimation);
        base.OnHandleDestroyed(e);
    }

    private bool HasShown;
    private void FormShown(object sender, EventArgs e)
    {
        if (_ControlMode || HasShown)
            return;

        if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
        {
            Rectangle SB = Screen.PrimaryScreen.Bounds;
            Rectangle CB = ParentForm.Bounds;
            ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
        }

        HasShown = true;
    }


    #region " Size Handling "

    private Rectangle Frame;
    protected override sealed void OnSizeChanged(EventArgs e)
    {
        if (_Movable && !_ControlMode)
        {
            Frame = new Rectangle(7, 7, Width - 14, _Header - 7);
        }

        InvalidateBitmap();
        Invalidate();

        base.OnSizeChanged(e);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (!(_LockWidth == 0))
            width = _LockWidth;
        if (!(_LockHeight == 0))
            height = _LockHeight;
        base.SetBoundsCore(x, y, width, height, specified);
    }

    #endregion

    #region " State Handling "

    protected MouseState State;
    private void SetState(MouseState current)
    {
        State = current;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
        {
            if (_Sizable && !_ControlMode)
                InvalidateMouse();
        }

        base.OnMouseMove(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        if (Enabled)
            SetState(MouseState.None);
        else
            SetState(MouseState.Block);
        base.OnEnabledChanged(e);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        SetState(MouseState.Over);
        base.OnMouseEnter(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        SetState(MouseState.Over);
        base.OnMouseUp(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        SetState(MouseState.None);

        if (GetChildAtPoint(PointToClient(MousePosition)) != null)
        {
            if (_Sizable && !_ControlMode)
            {
                Cursor = Cursors.Default;
                Previous = 0;
            }
        }

        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
            SetState(MouseState.Down);

        if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
        {
            if (_Movable && Frame.Contains(e.Location))
            {
                Capture = false;
                WM_LMBUTTONDOWN = true;
                DefWndProc(ref Messages[0]);
            }
            else if (_Sizable && !(Previous == 0))
            {
                Capture = false;
                WM_LMBUTTONDOWN = true;
                DefWndProc(ref Messages[Previous]);
            }
        }

        base.OnMouseDown(e);
    }

    private bool WM_LMBUTTONDOWN;
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (WM_LMBUTTONDOWN && m.Msg == 513)
        {
            WM_LMBUTTONDOWN = false;

            SetState(MouseState.Over);
            if (!_SmartBounds)
                return;

            if (IsParentMdi)
            {
                CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
            }
            else
            {
                CorrectBounds(Screen.FromControl(Parent).WorkingArea);
            }
        }
    }

    private Point GetIndexPoint;
    private bool B1;
    private bool B2;
    private bool B3;
    private bool B4;
    private int GetIndex()
    {
        GetIndexPoint = PointToClient(MousePosition);
        B1 = GetIndexPoint.X < 7;
        B2 = GetIndexPoint.X > Width - 7;
        B3 = GetIndexPoint.Y < 7;
        B4 = GetIndexPoint.Y > Height - 7;

        if (B1 && B3)
            return 4;
        if (B1 && B4)
            return 7;
        if (B2 && B3)
            return 5;
        if (B2 && B4)
            return 8;
        if (B1)
            return 1;
        if (B2)
            return 2;
        if (B3)
            return 3;
        if (B4)
            return 6;
        return 0;
    }

    private int Current;
    private int Previous;
    private void InvalidateMouse()
    {
        Current = GetIndex();
        if (Current == Previous)
            return;

        Previous = Current;
        switch (Previous)
        {
            case 0:
                Cursor = Cursors.Default;
                break;
            case 1:
            case 2:
                Cursor = Cursors.SizeWE;
                break;
            case 3:
            case 6:
                Cursor = Cursors.SizeNS;
                break;
            case 4:
            case 8:
                Cursor = Cursors.SizeNWSE;
                break;
            case 5:
            case 7:
                Cursor = Cursors.SizeNESW;
                break;
        }
    }

    private Message[] Messages = new Message[9];
    private void InitializeMessages()
    {
        Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
        for (int I = 1; I <= 8; I++)
        {
            Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
        }
    }

    private void CorrectBounds(Rectangle bounds)
    {
        if (Parent.Width > bounds.Width)
            Parent.Width = bounds.Width;
        if (Parent.Height > bounds.Height)
            Parent.Height = bounds.Height;

        int X = Parent.Location.X;
        int Y = Parent.Location.Y;

        if (X < bounds.X)
            X = bounds.X;
        if (Y < bounds.Y)
            Y = bounds.Y;

        int Width = bounds.X + bounds.Width;
        int Height = bounds.Y + bounds.Height;

        if (X + Parent.Width > Width)
            X = Width - Parent.Width;
        if (Y + Parent.Height > Height)
            Y = Height - Parent.Height;

        Parent.Location = new Point(X, Y);
    }

    #endregion


    #region " Base Properties "

    public override DockStyle Dock
    {
        get { return base.Dock; }
        set
        {
            if (!_ControlMode)
                return;
            base.Dock = value;
        }
    }

    private bool _BackColor;
    [Category("Misc")]
    public override Color BackColor
    {
        get { return base.BackColor; }
        set
        {
            if (value == base.BackColor)
                return;

            if (!IsHandleCreated && _ControlMode && value == Color.Transparent)
            {
                _BackColor = true;
                return;
            }

            base.BackColor = value;
            if (Parent != null)
            {
                if (!_ControlMode)
                    Parent.BackColor = value;
                ColorHook();
            }
        }
    }

    public override Size MinimumSize
    {
        get { return base.MinimumSize; }
        set
        {
            base.MinimumSize = value;
            if (Parent != null)
                Parent.MinimumSize = value;
        }
    }

    public override Size MaximumSize
    {
        get { return base.MaximumSize; }
        set
        {
            base.MaximumSize = value;
            if (Parent != null)
                Parent.MaximumSize = value;
        }
    }

    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            Invalidate();
        }
    }

    public override Font Font
    {
        get { return base.Font; }
        set
        {
            base.Font = value;
            Invalidate();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get { return Color.Empty; }
        set { }
    }
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image BackgroundImage
    {
        get { return null; }
        set { }
    }
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get { return ImageLayout.None; }
        set { }
    }

    #endregion

    #region " Public Properties "

    private bool _SmartBounds = true;
    public bool SmartBounds
    {
        get { return _SmartBounds; }
        set { _SmartBounds = value; }
    }

    private bool _Movable = true;
    public bool Movable
    {
        get { return _Movable; }
        set { _Movable = value; }
    }

    private bool _Sizable = true;
    public bool Sizable
    {
        get { return _Sizable; }
        set { _Sizable = value; }
    }

    private Color _TransparencyKey;
    public Color TransparencyKey
    {
        get
        {
            if (_IsParentForm && !_ControlMode)
                return ParentForm.TransparencyKey;
            else
                return _TransparencyKey;
        }
        set
        {
            if (value == _TransparencyKey)
                return;
            _TransparencyKey = value;

            if (_IsParentForm && !_ControlMode)
            {
                ParentForm.TransparencyKey = value;
                ColorHook();
            }
        }
    }

    private FormBorderStyle _BorderStyle;
    public FormBorderStyle BorderStyle
    {
        get
        {
            if (_IsParentForm && !_ControlMode)
                return ParentForm.FormBorderStyle;
            else
                return _BorderStyle;
        }
        set
        {
            _BorderStyle = value;

            if (_IsParentForm && !_ControlMode)
            {
                ParentForm.FormBorderStyle = value;

                if (!(value == FormBorderStyle.None))
                {
                    Movable = false;
                    Sizable = false;
                }
            }
        }
    }

    private FormStartPosition _StartPosition;
    public FormStartPosition StartPosition
    {
        get
        {
            if (_IsParentForm && !_ControlMode)
                return ParentForm.StartPosition;
            else
                return _StartPosition;
        }
        set
        {
            _StartPosition = value;

            if (_IsParentForm && !_ControlMode)
            {
                ParentForm.StartPosition = value;
            }
        }
    }

    private bool _NoRounding;
    public bool NoRounding
    {
        get { return _NoRounding; }
        set
        {
            _NoRounding = value;
            Invalidate();
        }
    }

    private Image _Image;
    public Image Image
    {
        get { return _Image; }
        set
        {
            if (value == null)
                _ImageSize = Size.Empty;
            else
                _ImageSize = value.Size;

            _Image = value;
            Invalidate();
        }
    }

    private Dictionary<string, Color> Items = new Dictionary<string, Color>();
    public Bloom[] Colors
    {
        get
        {
            List<Bloom> T = new List<Bloom>();
            Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

            while (E.MoveNext())
            {
                T.Add(new Bloom(E.Current.Key, E.Current.Value));
            }

            return T.ToArray();
        }
        set
        {
            foreach (Bloom B in value)
            {
                if (Items.ContainsKey(B.Name))
                    Items[B.Name] = B.Value;
            }

            InvalidateCustimization();
            ColorHook();
            Invalidate();
        }
    }

    private string _Customization;
    public string Customization
    {
        get { return _Customization; }
        set
        {
            if (value == _Customization)
                return;

            byte[] Data = null;
            Bloom[] Items = Colors;

            try
            {
                Data = Convert.FromBase64String(value);
                for (int I = 0; I <= Items.Length - 1; I++)
                {
                    Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
                }
            }
            catch
            {
                return;
            }

            _Customization = value;

            Colors = Items;
            ColorHook();
            Invalidate();
        }
    }

    private bool _Transparent;
    public bool Transparent
    {
        get { return _Transparent; }
        set
        {
            _Transparent = value;
            if (!(IsHandleCreated || _ControlMode))
                return;

            if (!value && !(BackColor.A == 255))
            {
                throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
            }

            SetStyle(ControlStyles.Opaque, !value);
            SetStyle(ControlStyles.SupportsTransparentBackColor, value);

            InvalidateBitmap();
            Invalidate();
        }
    }

    #endregion

    #region " Private Properties "

    private Size _ImageSize;
    protected Size ImageSize
    {
        get { return _ImageSize; }
    }

    private bool _IsParentForm;
    protected bool IsParentForm
    {
        get { return _IsParentForm; }
    }

    protected bool IsParentMdi
    {
        get
        {
            if (Parent == null)
                return false;
            return Parent.Parent != null;
        }
    }

    private int _LockWidth;
    protected int LockWidth
    {
        get { return _LockWidth; }
        set
        {
            _LockWidth = value;
            if (!(LockWidth == 0) && IsHandleCreated)
                Width = LockWidth;
        }
    }

    private int _LockHeight;
    protected int LockHeight
    {
        get { return _LockHeight; }
        set
        {
            _LockHeight = value;
            if (!(LockHeight == 0) && IsHandleCreated)
                Height = LockHeight;
        }
    }

    private int _Header = 24;
    protected int Header
    {
        get { return _Header; }
        set
        {
            _Header = value;

            if (!_ControlMode)
            {
                Frame = new Rectangle(7, 7, Width - 14, value - 7);
                Invalidate();
            }
        }
    }

    private bool _ControlMode;
    protected bool ControlMode
    {
        get { return _ControlMode; }
        set
        {
            _ControlMode = value;

            Transparent = _Transparent;
            if (_Transparent && _BackColor)
                BackColor = Color.Transparent;

            InvalidateBitmap();
            Invalidate();
        }
    }

    private bool _IsAnimated;
    protected bool IsAnimated
    {
        get { return _IsAnimated; }
        set
        {
            _IsAnimated = value;
            InvalidateTimer();
        }
    }

    #endregion


    #region " Property Helpers "

    protected Pen GetPen(string name)
    {
        return new Pen(Items[name]);
    }
    protected Pen GetPen(string name, float width)
    {
        return new Pen(Items[name], width);
    }

    protected SolidBrush GetBrush(string name)
    {
        return new SolidBrush(Items[name]);
    }

    protected Color GetColor(string name)
    {
        return Items[name];
    }

    protected void SetColor(string name, Color value)
    {
        if (Items.ContainsKey(name))
            Items[name] = value;
        else
            Items.Add(name, value);
    }
    protected void SetColor(string name, byte r, byte g, byte b)
    {
        SetColor(name, Color.FromArgb(r, g, b));
    }
    protected void SetColor(string name, byte a, byte r, byte g, byte b)
    {
        SetColor(name, Color.FromArgb(a, r, g, b));
    }
    protected void SetColor(string name, byte a, Color value)
    {
        SetColor(name, Color.FromArgb(a, value));
    }

    private void InvalidateBitmap()
    {
        if (_Transparent && _ControlMode)
        {
            if (Width == 0 || Height == 0)
                return;
            B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            G = Graphics.FromImage(B);
        }
        else
        {
            G = null;
            B = null;
        }
    }

    private void InvalidateCustimization()
    {
        MemoryStream M = new MemoryStream(Items.Count * 4);

        foreach (Bloom B in Colors)
        {
            M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
        }

        M.Close();
        _Customization = Convert.ToBase64String(M.ToArray());
    }

    private void InvalidateTimer()
    {
        if (DesignMode || !DoneCreation)
            return;

        if (_IsAnimated)
        {
            ThemeShare.AddAnimationCallback(DoAnimation);
        }
        else
        {
            ThemeShare.RemoveAnimationCallback(DoAnimation);
        }
    }

    #endregion


    #region " User Hooks "

    protected abstract void ColorHook();
    protected abstract void PaintHook();

    protected virtual void OnCreation()
    {
    }

    protected virtual void OnAnimation()
    {
    }

    #endregion


    #region " Offset "

    private Rectangle OffsetReturnRectangle;
    protected Rectangle Offset(Rectangle r, int amount)
    {
        OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
        return OffsetReturnRectangle;
    }

    private Size OffsetReturnSize;
    protected Size Offset(Size s, int amount)
    {
        OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
        return OffsetReturnSize;
    }

    private Point OffsetReturnPoint;
    protected Point Offset(Point p, int amount)
    {
        OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
        return OffsetReturnPoint;
    }

    #endregion

    #region " Center "


    private Point CenterReturn;
    protected Point Center(Rectangle p, Rectangle c)
    {
        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
        return CenterReturn;
    }
    protected Point Center(Rectangle p, Size c)
    {
        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
        return CenterReturn;
    }

    protected Point Center(Rectangle child)
    {
        return Center(Width, Height, child.Width, child.Height);
    }
    protected Point Center(Size child)
    {
        return Center(Width, Height, child.Width, child.Height);
    }
    protected Point Center(int childWidth, int childHeight)
    {
        return Center(Width, Height, childWidth, childHeight);
    }

    protected Point Center(Size p, Size c)
    {
        return Center(p.Width, p.Height, c.Width, c.Height);
    }

    protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
    {
        CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
        return CenterReturn;
    }

    #endregion

    #region " Measure "

    private Bitmap MeasureBitmap;

    private Graphics MeasureGraphics;
    protected Size Measure()
    {
        lock (MeasureGraphics)
        {
            return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
        }
    }
    protected Size Measure(string text)
    {
        lock (MeasureGraphics)
        {
            return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
        }
    }

    #endregion


    #region " DrawPixel "


    private SolidBrush DrawPixelBrush;
    protected void DrawPixel(Color c1, int x, int y)
    {
        if (_Transparent)
        {
            B.SetPixel(x, y, c1);
        }
        else
        {
            DrawPixelBrush = new SolidBrush(c1);
            G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
        }
    }

    #endregion

    #region " DrawCorners "


    private SolidBrush DrawCornersBrush;
    protected void DrawCorners(Color c1, int offset)
    {
        DrawCorners(c1, 0, 0, Width, Height, offset);
    }
    protected void DrawCorners(Color c1, Rectangle r1, int offset)
    {
        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
    }
    protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
    {
        DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    }

    protected void DrawCorners(Color c1)
    {
        DrawCorners(c1, 0, 0, Width, Height);
    }
    protected void DrawCorners(Color c1, Rectangle r1)
    {
        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
    }
    protected void DrawCorners(Color c1, int x, int y, int width, int height)
    {
        if (_NoRounding)
            return;

        if (_Transparent)
        {
            B.SetPixel(x, y, c1);
            B.SetPixel(x + (width - 1), y, c1);
            B.SetPixel(x, y + (height - 1), c1);
            B.SetPixel(x + (width - 1), y + (height - 1), c1);
        }
        else
        {
            DrawCornersBrush = new SolidBrush(c1);
            G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
            G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
            G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
            G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
        }
    }

    #endregion

    #region " DrawBorders "

    protected void DrawBorders(Pen p1, int offset)
    {
        DrawBorders(p1, 0, 0, Width, Height, offset);
    }
    protected void DrawBorders(Pen p1, Rectangle r, int offset)
    {
        DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
    }
    protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
    {
        DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    }

    protected void DrawBorders(Pen p1)
    {
        DrawBorders(p1, 0, 0, Width, Height);
    }
    protected void DrawBorders(Pen p1, Rectangle r)
    {
        DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
    }
    protected void DrawBorders(Pen p1, int x, int y, int width, int height)
    {
        G.DrawRectangle(p1, x, y, width - 1, height - 1);
    }

    #endregion

    #region " DrawText "

    private Point DrawTextPoint;

    private Size DrawTextSize;
    protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
    {
        DrawText(b1, Text, a, x, y);
    }
    protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
    {
        if (text.Length == 0)
            return;

        DrawTextSize = Measure(text);
        DrawTextPoint = new Point(Width / 2 - DrawTextSize.Width / 2, Header / 2 - DrawTextSize.Height / 2);

        switch (a)
        {
            case HorizontalAlignment.Left:
                G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
                break;
            case HorizontalAlignment.Center:
                G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                break;
            case HorizontalAlignment.Right:
                G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                break;
        }
    }

    protected void DrawText(Brush b1, Point p1)
    {
        if (Text.Length == 0)
            return;
        G.DrawString(Text, Font, b1, p1);
    }
    protected void DrawText(Brush b1, int x, int y)
    {
        if (Text.Length == 0)
            return;
        G.DrawString(Text, Font, b1, x, y);
    }

    #endregion

    #region " DrawImage "


    private Point DrawImagePoint;
    protected void DrawImage(HorizontalAlignment a, int x, int y)
    {
        DrawImage(_Image, a, x, y);
    }
    protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
    {
        if (image == null)
            return;
        DrawImagePoint = new Point(Width / 2 - image.Width / 2, Header / 2 - image.Height / 2);

        switch (a)
        {
            case HorizontalAlignment.Left:
                G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
                break;
            case HorizontalAlignment.Center:
                G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
                break;
            case HorizontalAlignment.Right:
                G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
                break;
        }
    }

    protected void DrawImage(Point p1)
    {
        DrawImage(_Image, p1.X, p1.Y);
    }
    protected void DrawImage(int x, int y)
    {
        DrawImage(_Image, x, y);
    }

    protected void DrawImage(Image image, Point p1)
    {
        DrawImage(image, p1.X, p1.Y);
    }
    protected void DrawImage(Image image, int x, int y)
    {
        if (image == null)
            return;
        G.DrawImage(image, x, y, image.Width, image.Height);
    }

    #endregion

    #region " DrawGradient "

    private LinearGradientBrush DrawGradientBrush;

    private Rectangle DrawGradientRectangle;
    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(blend, DrawGradientRectangle);
    }
    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(blend, DrawGradientRectangle, angle);
    }

    protected void DrawGradient(ColorBlend blend, Rectangle r)
    {
        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
        DrawGradientBrush.InterpolationColors = blend;
        G.FillRectangle(DrawGradientBrush, r);
    }
    protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
    {
        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
        DrawGradientBrush.InterpolationColors = blend;
        G.FillRectangle(DrawGradientBrush, r);
    }


    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(c1, c2, DrawGradientRectangle);
    }
    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(c1, c2, DrawGradientRectangle, angle);
    }

    protected void DrawGradient(Color c1, Color c2, Rectangle r)
    {
        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
        G.FillRectangle(DrawGradientBrush, r);
    }
    protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
    {
        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
        G.FillRectangle(DrawGradientBrush, r);
    }

    #endregion

    #region " DrawRadial "

    private GraphicsPath DrawRadialPath;
    private PathGradientBrush DrawRadialBrush1;
    private LinearGradientBrush DrawRadialBrush2;

    private Rectangle DrawRadialRectangle;
    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
    }
    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
    }
    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(blend, DrawRadialRectangle, cx, cy);
    }

    public void DrawRadial(ColorBlend blend, Rectangle r)
    {
        DrawRadial(blend, r, r.Width / 2, r.Height / 2);
    }
    public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
    {
        DrawRadial(blend, r, center.X, center.Y);
    }
    public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
    {
        DrawRadialPath.Reset();
        DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

        DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
        DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
        DrawRadialBrush1.InterpolationColors = blend;

        if (G.SmoothingMode == SmoothingMode.AntiAlias)
        {
            G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
        }
        else
        {
            G.FillEllipse(DrawRadialBrush1, r);
        }
    }


    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(c1, c2, DrawGradientRectangle);
    }
    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(c1, c2, DrawGradientRectangle, angle);
    }

    protected void DrawRadial(Color c1, Color c2, Rectangle r)
    {
        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
        G.FillRectangle(DrawGradientBrush, r);
    }
    protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
    {
        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
        G.FillEllipse(DrawGradientBrush, r);
    }

    #endregion

    #region " CreateRound "

    private GraphicsPath CreateRoundPath;

    private Rectangle CreateRoundRectangle;
    public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
    {
        CreateRoundRectangle = new Rectangle(x, y, width, height);
        return CreateRound(CreateRoundRectangle, slope);
    }

    public GraphicsPath CreateRound(Rectangle r, int slope)
    {
        CreateRoundPath = new GraphicsPath(FillMode.Winding);
        CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
        CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
        CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
        CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
        CreateRoundPath.CloseFigure();
        return CreateRoundPath;
    }

    #endregion

}

abstract class ThemeControl154 : Control
{


    #region " Initialization "

    protected Graphics G;

    protected Bitmap B;
    public ThemeControl154()
    {
        SetStyle((ControlStyles)139270, true);

        _ImageSize = Size.Empty;
        Font = new Font("Verdana", 8);

        MeasureBitmap = new Bitmap(1, 1);
        MeasureGraphics = Graphics.FromImage(MeasureBitmap);

        DrawRadialPath = new GraphicsPath();

        InvalidateCustimization();
        //Remove?
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        InvalidateCustimization();
        ColorHook();

        if (!(_LockWidth == 0))
            Width = _LockWidth;
        if (!(_LockHeight == 0))
            Height = _LockHeight;

        Transparent = _Transparent;
        if (_Transparent && _BackColor)
            BackColor = Color.Transparent;

        base.OnHandleCreated(e);
    }

    private bool DoneCreation;
    protected override sealed void OnParentChanged(EventArgs e)
    {
        if (Parent != null)
        {
            OnCreation();
            DoneCreation = true;
            InvalidateTimer();
        }

        base.OnParentChanged(e);
    }

    #endregion

    private void DoAnimation(bool i)
    {
        OnAnimation();
        if (i)
            Invalidate();
    }

    protected override sealed void OnPaint(PaintEventArgs e)
    {
        if (Width == 0 || Height == 0)
            return;

        if (_Transparent)
        {
            PaintHook();
            e.Graphics.DrawImage(B, 0, 0);
        }
        else
        {
            G = e.Graphics;
            PaintHook();
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        ThemeShare.RemoveAnimationCallback(DoAnimation);
        base.OnHandleDestroyed(e);
    }

    #region " Size Handling "

    protected override sealed void OnSizeChanged(EventArgs e)
    {
        if (_Transparent)
        {
            InvalidateBitmap();
        }

        Invalidate();
        base.OnSizeChanged(e);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (!(_LockWidth == 0))
            width = _LockWidth;
        if (!(_LockHeight == 0))
            height = _LockHeight;
        base.SetBoundsCore(x, y, width, height, specified);
    }

    #endregion

    #region " State Handling "

    private bool InPosition;
    protected override void OnMouseEnter(EventArgs e)
    {
        InPosition = true;
        SetState(MouseState.Over);
        base.OnMouseEnter(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (InPosition)
            SetState(MouseState.Over);
        base.OnMouseUp(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
            SetState(MouseState.Down);
        base.OnMouseDown(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        InPosition = false;
        SetState(MouseState.None);
        base.OnMouseLeave(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        if (Enabled)
            SetState(MouseState.None);
        else
            SetState(MouseState.Block);
        base.OnEnabledChanged(e);
    }

    protected MouseState State;
    private void SetState(MouseState current)
    {
        State = current;
        Invalidate();
    }

    #endregion


    #region " Base Properties "

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get { return Color.Empty; }
        set { }
    }
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image BackgroundImage
    {
        get { return null; }
        set { }
    }
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get { return ImageLayout.None; }
        set { }
    }

    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            Invalidate();
        }
    }
    public override Font Font
    {
        get { return base.Font; }
        set
        {
            base.Font = value;
            Invalidate();
        }
    }

    private bool _BackColor;
    [Category("Misc")]
    public override Color BackColor
    {
        get { return base.BackColor; }
        set
        {
            if (!IsHandleCreated && value == Color.Transparent)
            {
                _BackColor = true;
                return;
            }

            base.BackColor = value;
            if (Parent != null)
                ColorHook();
        }
    }

    #endregion

    #region " Public Properties "

    private bool _NoRounding;
    public bool NoRounding
    {
        get { return _NoRounding; }
        set
        {
            _NoRounding = value;
            Invalidate();
        }
    }

    private Image _Image;
    public Image Image
    {
        get { return _Image; }
        set
        {
            if (value == null)
            {
                _ImageSize = Size.Empty;
            }
            else
            {
                _ImageSize = value.Size;
            }

            _Image = value;
            Invalidate();
        }
    }

    private bool _Transparent;
    public bool Transparent
    {
        get { return _Transparent; }
        set
        {
            _Transparent = value;
            if (!IsHandleCreated)
                return;

            if (!value && !(BackColor.A == 255))
            {
                throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
            }

            SetStyle(ControlStyles.Opaque, !value);
            SetStyle(ControlStyles.SupportsTransparentBackColor, value);

            if (value)
                InvalidateBitmap();
            else
                B = null;
            Invalidate();
        }
    }

    private Dictionary<string, Color> Items = new Dictionary<string, Color>();
    public Bloom[] Colors
    {
        get
        {
            List<Bloom> T = new List<Bloom>();
            Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

            while (E.MoveNext())
            {
                T.Add(new Bloom(E.Current.Key, E.Current.Value));
            }

            return T.ToArray();
        }
        set
        {
            foreach (Bloom B in value)
            {
                if (Items.ContainsKey(B.Name))
                    Items[B.Name] = B.Value;
            }

            InvalidateCustimization();
            ColorHook();
            Invalidate();
        }
    }

    private string _Customization;
    public string Customization
    {
        get { return _Customization; }
        set
        {
            if (value == _Customization)
                return;

            byte[] Data = null;
            Bloom[] Items = Colors;

            try
            {
                Data = Convert.FromBase64String(value);
                for (int I = 0; I <= Items.Length - 1; I++)
                {
                    Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
                }
            }
            catch
            {
                return;
            }

            _Customization = value;

            Colors = Items;
            ColorHook();
            Invalidate();
        }
    }

    #endregion

    #region " Private Properties "

    private Size _ImageSize;
    protected Size ImageSize
    {
        get { return _ImageSize; }
    }

    private int _LockWidth;
    protected int LockWidth
    {
        get { return _LockWidth; }
        set
        {
            _LockWidth = value;
            if (!(LockWidth == 0) && IsHandleCreated)
                Width = LockWidth;
        }
    }

    private int _LockHeight;
    protected int LockHeight
    {
        get { return _LockHeight; }
        set
        {
            _LockHeight = value;
            if (!(LockHeight == 0) && IsHandleCreated)
                Height = LockHeight;
        }
    }

    private bool _IsAnimated;
    protected bool IsAnimated
    {
        get { return _IsAnimated; }
        set
        {
            _IsAnimated = value;
            InvalidateTimer();
        }
    }

    #endregion


    #region " Property Helpers "

    protected Pen GetPen(string name)
    {
        return new Pen(Items[name]);
    }
    protected Pen GetPen(string name, float width)
    {
        return new Pen(Items[name], width);
    }

    protected SolidBrush GetBrush(string name)
    {
        return new SolidBrush(Items[name]);
    }

    protected Color GetColor(string name)
    {
        return Items[name];
    }

    protected void SetColor(string name, Color value)
    {
        if (Items.ContainsKey(name))
            Items[name] = value;
        else
            Items.Add(name, value);
    }
    protected void SetColor(string name, byte r, byte g, byte b)
    {
        SetColor(name, Color.FromArgb(r, g, b));
    }
    protected void SetColor(string name, byte a, byte r, byte g, byte b)
    {
        SetColor(name, Color.FromArgb(a, r, g, b));
    }
    protected void SetColor(string name, byte a, Color value)
    {
        SetColor(name, Color.FromArgb(a, value));
    }

    private void InvalidateBitmap()
    {
        if (Width == 0 || Height == 0)
            return;
        B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
        G = Graphics.FromImage(B);
    }

    private void InvalidateCustimization()
    {
        MemoryStream M = new MemoryStream(Items.Count * 4);

        foreach (Bloom B in Colors)
        {
            M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
        }

        M.Close();
        _Customization = Convert.ToBase64String(M.ToArray());
    }

    private void InvalidateTimer()
    {
        if (DesignMode || !DoneCreation)
            return;

        if (_IsAnimated)
        {
            ThemeShare.AddAnimationCallback(DoAnimation);
        }
        else
        {
            ThemeShare.RemoveAnimationCallback(DoAnimation);
        }
    }
    #endregion


    #region " User Hooks "

    protected abstract void ColorHook();
    protected abstract void PaintHook();

    protected virtual void OnCreation()
    {
    }

    protected virtual void OnAnimation()
    {
    }

    #endregion


    #region " Offset "

    private Rectangle OffsetReturnRectangle;
    protected Rectangle Offset(Rectangle r, int amount)
    {
        OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
        return OffsetReturnRectangle;
    }

    private Size OffsetReturnSize;
    protected Size Offset(Size s, int amount)
    {
        OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
        return OffsetReturnSize;
    }

    private Point OffsetReturnPoint;
    protected Point Offset(Point p, int amount)
    {
        OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
        return OffsetReturnPoint;
    }

    #endregion

    #region " Center "


    private Point CenterReturn;
    protected Point Center(Rectangle p, Rectangle c)
    {
        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
        return CenterReturn;
    }
    protected Point Center(Rectangle p, Size c)
    {
        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
        return CenterReturn;
    }

    protected Point Center(Rectangle child)
    {
        return Center(Width, Height, child.Width, child.Height);
    }
    protected Point Center(Size child)
    {
        return Center(Width, Height, child.Width, child.Height);
    }
    protected Point Center(int childWidth, int childHeight)
    {
        return Center(Width, Height, childWidth, childHeight);
    }

    protected Point Center(Size p, Size c)
    {
        return Center(p.Width, p.Height, c.Width, c.Height);
    }

    protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
    {
        CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
        return CenterReturn;
    }

    #endregion

    #region " Measure "

    private Bitmap MeasureBitmap;
    //TODO: Potential issues during multi-threading.
    private Graphics MeasureGraphics;

    protected Size Measure()
    {
        return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
    }
    protected Size Measure(string text)
    {
        return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
    }

    #endregion


    #region " DrawPixel "


    private SolidBrush DrawPixelBrush;
    protected void DrawPixel(Color c1, int x, int y)
    {
        if (_Transparent)
        {
            B.SetPixel(x, y, c1);
        }
        else
        {
            DrawPixelBrush = new SolidBrush(c1);
            G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
        }
    }

    #endregion

    #region " DrawCorners "


    private SolidBrush DrawCornersBrush;
    protected void DrawCorners(Color c1, int offset)
    {
        DrawCorners(c1, 0, 0, Width, Height, offset);
    }
    protected void DrawCorners(Color c1, Rectangle r1, int offset)
    {
        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
    }
    protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
    {
        DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    }

    protected void DrawCorners(Color c1)
    {
        DrawCorners(c1, 0, 0, Width, Height);
    }
    protected void DrawCorners(Color c1, Rectangle r1)
    {
        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
    }
    protected void DrawCorners(Color c1, int x, int y, int width, int height)
    {
        if (_NoRounding)
            return;

        if (_Transparent)
        {
            B.SetPixel(x, y, c1);
            B.SetPixel(x + (width - 1), y, c1);
            B.SetPixel(x, y + (height - 1), c1);
            B.SetPixel(x + (width - 1), y + (height - 1), c1);
        }
        else
        {
            DrawCornersBrush = new SolidBrush(c1);
            G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
            G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
            G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
            G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
        }
    }

    #endregion

    #region " DrawBorders "

    protected void DrawBorders(Pen p1, int offset)
    {
        DrawBorders(p1, 0, 0, Width, Height, offset);
    }
    protected void DrawBorders(Pen p1, Rectangle r, int offset)
    {
        DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
    }
    protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
    {
        DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    }

    protected void DrawBorders(Pen p1)
    {
        DrawBorders(p1, 0, 0, Width, Height);
    }
    protected void DrawBorders(Pen p1, Rectangle r)
    {
        DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
    }
    protected void DrawBorders(Pen p1, int x, int y, int width, int height)
    {
        G.DrawRectangle(p1, x, y, width - 1, height - 1);
    }

    #endregion

    #region " DrawText "

    private Point DrawTextPoint;

    private Size DrawTextSize;
    protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
    {
        DrawText(b1, Text, a, x, y);
    }
    protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
    {
        if (text.Length == 0)
            return;

        DrawTextSize = Measure(text);
        DrawTextPoint = Center(DrawTextSize);

        switch (a)
        {
            case HorizontalAlignment.Left:
                G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
                break;
            case HorizontalAlignment.Center:
                G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                break;
            case HorizontalAlignment.Right:
                G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                break;
        }
    }

    protected void DrawText(Brush b1, Point p1)
    {
        if (Text.Length == 0)
            return;
        G.DrawString(Text, Font, b1, p1);
    }
    protected void DrawText(Brush b1, int x, int y)
    {
        if (Text.Length == 0)
            return;
        G.DrawString(Text, Font, b1, x, y);
    }

    #endregion

    #region " DrawImage "


    private Point DrawImagePoint;
    protected void DrawImage(HorizontalAlignment a, int x, int y)
    {
        DrawImage(_Image, a, x, y);
    }
    protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
    {
        if (image == null)
            return;
        DrawImagePoint = Center(image.Size);

        switch (a)
        {
            case HorizontalAlignment.Left:
                G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
                break;
            case HorizontalAlignment.Center:
                G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
                break;
            case HorizontalAlignment.Right:
                G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
                break;
        }
    }

    protected void DrawImage(Point p1)
    {
        DrawImage(_Image, p1.X, p1.Y);
    }
    protected void DrawImage(int x, int y)
    {
        DrawImage(_Image, x, y);
    }

    protected void DrawImage(Image image, Point p1)
    {
        DrawImage(image, p1.X, p1.Y);
    }
    protected void DrawImage(Image image, int x, int y)
    {
        if (image == null)
            return;
        G.DrawImage(image, x, y, image.Width, image.Height);
    }

    #endregion

    #region " DrawGradient "

    private LinearGradientBrush DrawGradientBrush;

    private Rectangle DrawGradientRectangle;
    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(blend, DrawGradientRectangle);
    }
    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(blend, DrawGradientRectangle, angle);
    }

    protected void DrawGradient(ColorBlend blend, Rectangle r)
    {
        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
        DrawGradientBrush.InterpolationColors = blend;
        G.FillRectangle(DrawGradientBrush, r);
    }
    protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
    {
        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
        DrawGradientBrush.InterpolationColors = blend;
        G.FillRectangle(DrawGradientBrush, r);
    }


    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(c1, c2, DrawGradientRectangle);
    }
    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
    {
        DrawGradientRectangle = new Rectangle(x, y, width, height);
        DrawGradient(c1, c2, DrawGradientRectangle, angle);
    }

    protected void DrawGradient(Color c1, Color c2, Rectangle r)
    {
        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
        G.FillRectangle(DrawGradientBrush, r);
    }
    protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
    {
        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
        G.FillRectangle(DrawGradientBrush, r);
    }

    #endregion

    #region " DrawRadial "

    private GraphicsPath DrawRadialPath;
    private PathGradientBrush DrawRadialBrush1;
    private LinearGradientBrush DrawRadialBrush2;

    private Rectangle DrawRadialRectangle;
    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
    }
    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
    }
    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(blend, DrawRadialRectangle, cx, cy);
    }

    public void DrawRadial(ColorBlend blend, Rectangle r)
    {
        DrawRadial(blend, r, r.Width / 2, r.Height / 2);
    }
    public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
    {
        DrawRadial(blend, r, center.X, center.Y);
    }
    public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
    {
        DrawRadialPath.Reset();
        DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

        DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
        DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
        DrawRadialBrush1.InterpolationColors = blend;

        if (G.SmoothingMode == SmoothingMode.AntiAlias)
        {
            G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
        }
        else
        {
            G.FillEllipse(DrawRadialBrush1, r);
        }
    }


    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(c1, c2, DrawRadialRectangle);
    }
    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
    {
        DrawRadialRectangle = new Rectangle(x, y, width, height);
        DrawRadial(c1, c2, DrawRadialRectangle, angle);
    }

    protected void DrawRadial(Color c1, Color c2, Rectangle r)
    {
        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
        G.FillEllipse(DrawRadialBrush2, r);
    }
    protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
    {
        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
        G.FillEllipse(DrawRadialBrush2, r);
    }

    #endregion

    #region " CreateRound "

    private GraphicsPath CreateRoundPath;

    private Rectangle CreateRoundRectangle;
    public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
    {
        CreateRoundRectangle = new Rectangle(x, y, width, height);
        return CreateRound(CreateRoundRectangle, slope);
    }

    public GraphicsPath CreateRound(Rectangle r, int slope)
    {
        CreateRoundPath = new GraphicsPath(FillMode.Winding);
        CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
        CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
        CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
        CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
        CreateRoundPath.CloseFigure();
        return CreateRoundPath;
    }

    #endregion

}

static class ThemeShare
{

    #region " Animation "

    private static int Frames;
    private static bool Invalidate;

    public static PrecisionTimer ThemeTimer = new PrecisionTimer();
    //1000 / 50 = 20 FPS
    private const int FPS = 50;

    private const int Rate = 10;
    public delegate void AnimationDelegate(bool invalidate);


    private static List<AnimationDelegate> Callbacks = new List<AnimationDelegate>();
    private static void HandleCallbacks(IntPtr state, bool reserve)
    {
        Invalidate = (Frames >= FPS);
        if (Invalidate)
            Frames = 0;

        lock (Callbacks)
        {
            for (int I = 0; I <= Callbacks.Count - 1; I++)
            {
                Callbacks[I].Invoke(Invalidate);
            }
        }

        Frames += Rate;
    }

    private static void InvalidateThemeTimer()
    {
        if (Callbacks.Count == 0)
        {
            ThemeTimer.Delete();
        }
        else
        {
            ThemeTimer.Create(0, Rate, HandleCallbacks);
        }
    }

    public static void AddAnimationCallback(AnimationDelegate callback)
    {
        lock (Callbacks)
        {
            if (Callbacks.Contains(callback))
                return;

            Callbacks.Add(callback);
            InvalidateThemeTimer();
        }
    }

    public static void RemoveAnimationCallback(AnimationDelegate callback)
    {
        lock (Callbacks)
        {
            if (!Callbacks.Contains(callback))
                return;

            Callbacks.Remove(callback);
            InvalidateThemeTimer();
        }
    }

    #endregion

}

enum MouseState : byte
{
    None = 0,
    Over = 1,
    Down = 2,
    Block = 3
}

struct Bloom
{

    public string _Name;
    public string Name
    {
        get { return _Name; }
    }

    private Color _Value;
    public Color Value
    {
        get { return _Value; }
        set { _Value = value; }
    }

    public string ValueHex
    {
        get { return string.Concat("#", _Value.R.ToString("X2", null), _Value.G.ToString("X2", null), _Value.B.ToString("X2", null)); }
        set
        {
            try
            {
                _Value = ColorTranslator.FromHtml(value);
            }
            catch
            {
                return;
            }
        }
    }


    public Bloom(string name, Color value)
    {
        _Name = name;
        _Value = value;
    }
}

//------------------
//Creator: aeonhack
//Site: elitevs.net
//Created: 11/30/2011
//Changed: 11/30/2011
//Version: 1.0.0
//------------------
class PrecisionTimer : IDisposable
{

    private bool _Enabled;
    public bool Enabled
    {
        get { return _Enabled; }
    }

    private IntPtr Handle;

    private TimerDelegate TimerCallback;
    [DllImport("kernel32.dll", EntryPoint = "CreateTimerQueueTimer")]
    private static extern bool CreateTimerQueueTimer(ref IntPtr handle, IntPtr queue, TimerDelegate callback, IntPtr state, uint dueTime, uint period, uint flags);

    [DllImport("kernel32.dll", EntryPoint = "DeleteTimerQueueTimer")]
    private static extern bool DeleteTimerQueueTimer(IntPtr queue, IntPtr handle, IntPtr callback);

    public delegate void TimerDelegate(IntPtr r1, bool r2);

    public void Create(uint dueTime, uint period, TimerDelegate callback)
    {
        if (_Enabled)
            return;

        TimerCallback = callback;
        bool Success = CreateTimerQueueTimer(ref Handle, IntPtr.Zero, TimerCallback, IntPtr.Zero, dueTime, period, 0);

        if (!Success)
            ThrowNewException("CreateTimerQueueTimer");
        _Enabled = Success;
    }

    public void Delete()
    {
        if (!_Enabled)
            return;
        bool Success = DeleteTimerQueueTimer(IntPtr.Zero, Handle, IntPtr.Zero);

        if (!Success && !(Marshal.GetLastWin32Error() == 997))
        {
            ThrowNewException("DeleteTimerQueueTimer");
        }

        _Enabled = !Success;
    }

    private void ThrowNewException(string name)
    {
        throw new Exception(string.Format("{0} failed. Win32Error: {1}", name, Marshal.GetLastWin32Error()));
    }

    public void Dispose()
    {
        Delete();
    }
}
#endregion

#region "Credits"
///<---------------------///
///<Creator TheEliteNoob>
///<Site:   myxkcd.co.cc>
///<Created:   13-2-2012>
///<Theme Base:    1.5.4>
///<---------------------///
///
#endregion

///<============================================================>
///<                     Start The Theme!                       >
///<============================================================>

#region Enums for Options
public enum MultiColor
{
    Black = 0,
    Green = 1,
    Blue = 2,
    Pink = 3
}

public enum HoverOptions
{
    Green = 0,
    Blue = 1,
    Pink = 2
}

public enum HatchColors
{
    White = 0,
    Green = 1,
    Blue = 2,
    Pink = 3
}

public enum IsHatched
{
    Yes = 0,
    No = 1
}
#endregion

#region Theme
class genesisTheme : ThemeContainer154 // White hatch color
{
    #region Text Location
    public enum TextLocation
    {
        Left = 0, //if there isn't an icon
        LeftIcon = 1, //if there is an icon
        Center = 2,
        CenterIcon = 3
    }

    TextLocation _location;

    public TextLocation TextPlacement
    {
        get
        {
            return this._location;
        }
        set
        {
            this._location = value;
            Invalidate();
        }
    }

    MultiColor _textcolor;

    public MultiColor TextColor
    {
        get
        {
            return this._textcolor;
        }
        set
        {
            this._textcolor = value;
            Invalidate();
        }
    }

    #endregion

    #region Hatch Options
    HatchColors _color;

    public HatchColors CurrentColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    IsHatched _yesno;

    public IsHatched HatchONOff
    {
        get
        {
            return this._yesno;
        }
        set
        {
            this._yesno = value;
            Invalidate();
        }
    }
    #endregion

    #region IconSettings
    private Icon IconLocation;
    public Icon CustomIcon
    {
        get
        {
            return IconLocation;
        }
        set
        {
            IconLocation = value;
        }
    }

    public enum UseIcon
    {
        No = 0,
        Yes = 1
    }

    UseIcon _iconyesno;

    public UseIcon UseAnIcon
    {
        get
        {
            return this._iconyesno;
        }
        set
        {
            this._iconyesno = value;
            Invalidate();
        }
    }
    #endregion

    public genesisTheme()
    {
        BackColor = Color.FromArgb(250, 250, 250);
        SetColor("Back", 250, 250, 250);
        SetColor("Gradient1", 254, 254, 254);
        SetColor("Gradient2", 243, 243, 243);
        SetColor("Border1", 240, 240, 240);
        SetColor("Border2", 254, 254, 254);
        SetColor("Border3", 200, 200, 200);
        SetColor("Line1", 250, 250, 250);
        SetColor("Line2", 240, 240, 240);
        SetColor("Shade1", 70, Color.White);
        SetColor("Shade2", Color.Transparent);
    }

    private Color C1;
    private Color C2;
    private Color C3;
    private Color C4;
    private Color C5;
    private Pen P1;
    private Pen P2;
    private Pen P3;
    private Pen P4;
    private Pen P5;
    private HatchBrush B1;
    private SolidBrush B2;

    protected override void ColorHook()
    {
        C1 = GetColor("Back");
        C2 = GetColor("Gradient1");
        C3 = GetColor("Gradient2");
        C4 = GetColor("Shade1");
        C5 = GetColor("Shade2");
        P1 = new Pen(GetColor("Border1"));
        P2 = new Pen(GetColor("Line1"));
        P3 = new Pen(GetColor("Line2"));
        P4 = new Pen(GetColor("Border2"));
        P5 = new Pen(GetColor("Border3"));
        BackColor = C1;
    }

    private Rectangle RT1;
    protected override void PaintHook()
    {
        G.Clear(C1);
        RT1 = new Rectangle(1, 1, Width - 2, 22);
        DrawGradient(C2, C3, RT1, 90f);
        DrawBorders(P1, RT1);
        G.DrawLine(P2, 0, 23, Width, 23);
        switch (_yesno)
        {
            case IsHatched.Yes:
                switch (_color)
                {
                    case HatchColors.White:
                        SetColor("Hatch1", 254, 254, 254);
                        SetColor("Hatch2", 248, 248, 248);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Blue:
                        SetColor("Hatch1", 78, 205, 196);
                        SetColor("Hatch2", 72, 199, 190);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Green:
                        SetColor("Hatch1", 199, 244, 100);
                        SetColor("Hatch2", 193, 238, 94);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Pink:
                        SetColor("Hatch1", 255, 107, 107);
                        SetColor("Hatch2", 249, 101, 101);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                }
                break;
            case IsHatched.No:
                switch (_color)
                {
                    case HatchColors.White:
                        SetColor("Hatch1", 254, 254, 254);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Blue:
                        SetColor("Hatch1", 78, 205, 196);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Green:
                        SetColor("Hatch1", 199, 244, 100);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Pink:
                        SetColor("Hatch1", 255, 107, 107);
                        SetColor("Hatch2", 249, 101, 101);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                }
                break;
        }
        switch (_textcolor)
        {
            case MultiColor.Black:
                SetColor("Text", 24, 24, 24);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Blue:
                SetColor("Text", 78, 205, 196);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Green:
                SetColor("Text", 199, 244, 100);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Pink:
                SetColor("Text", 255, 107, 107);
                B2 = new SolidBrush(GetColor("Text"));
                break;
        }
        G.FillRectangle(B1, 0, 24, Width, 13);
        DrawGradient(C4, C5, 0, 24, Width, 6);
        G.DrawLine(P3, 0, 37, Width, 37);
        DrawBorders(P4, 1, 38, Width - 2, Height - 39);
        switch (_iconyesno)
        {
            case UseIcon.Yes:
                Icon ico = new Icon(IconLocation, new Size(16, 16));
                switch (_location)
                {
                    case TextLocation.Left:
                        DrawText(B2, HorizontalAlignment.Left, 1, -2);
                        break;
                    case TextLocation.LeftIcon:
                        G.DrawIconUnstretched(((System.Drawing.Icon)(ico)), ((Rectangle)(RT1)));
                        DrawText(B2, HorizontalAlignment.Left, 18, -2);
                        break;
                    case TextLocation.Center:
                        DrawText(B2, HorizontalAlignment.Center, 0, -2);
                        break;
                    case TextLocation.CenterIcon:
                        G.DrawIconUnstretched(((System.Drawing.Icon)(ico)), ((Rectangle)(RT1)));
                        DrawText(B2, HorizontalAlignment.Center, 0, -2);
                        break;
                }
                break;
            case UseIcon.No:
                switch (_location)
                {
                    case TextLocation.Left:
                        DrawText(B2, HorizontalAlignment.Left, 1, -2);
                        break;
                    case TextLocation.LeftIcon:
                        DrawText(B2, HorizontalAlignment.Left, 1, -2);
                        break;
                    case TextLocation.Center:
                        DrawText(B2, HorizontalAlignment.Center, 0, -2);
                        break;
                    case TextLocation.CenterIcon:
                        DrawText(B2, HorizontalAlignment.Center, 0, -2);
                        break;
                }
                break;
        }
        DrawBorders(P5);
    }
}
#endregion

#region Button
class genesisButton : ThemeControl154 /// <The Button has 4 colors defined!>
{

    MultiColor _color;

    public MultiColor CurrentColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    public genesisButton()
    {
        SetColor("DownGradient1", 252, 252, 252);
        SetColor("DownGradient2", 238, 238, 238);
        SetColor("NoneGradient1", 240, 240, 240);
        SetColor("NoneGradient2", 254, 254, 254);
        SetColor("HoverGradient1", 238, 238, 238);
        SetColor("HoverGradient2", 252, 252, 252);
        SetColor("TextShade", 30, 35, 35, 35);
        SetColor("Border1", 225, 225, 225);
        SetColor("Border2", 235, 235, 235);
    }

    private Color C1;
    private Color C2;
    private Color C3;
    private Color C4;
    private Color C5;
    private Color C6;
    private SolidBrush B1;
    private SolidBrush B2;
    private Pen P1;
    private Pen P2;

    protected override void ColorHook()
    {
        C1 = GetColor("DownGradient1");
        C2 = GetColor("DownGradient2");
        C3 = GetColor("NoneGradient1");
        C4 = GetColor("NoneGradient2");
        C5 = GetColor("HoverGradient1");
        C6 = GetColor("HoverGradient2");
        B1 = new SolidBrush(GetColor("TextShade"));
        P1 = new Pen(GetColor("Border1"));
        P2 = new Pen(GetColor("Border2"));
    }


    protected override void PaintHook()
    {
        if (State == MouseState.Down)
        {
            DrawGradient(C2, C1, ClientRectangle, 90f);
        }
        else if (State == MouseState.Over)
        {
            DrawGradient(C6, C5, ClientRectangle, 90f);
        }
        else
        {
            DrawGradient(C4, C3, ClientRectangle, 90f);
        }
        DrawBorders(P1, 1);
        DrawBorders(P2);
        DrawCorners(BackColor);
        switch (_color)
        {
            case MultiColor.Black:
                SetColor("Text", 24, 24, 24);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Blue:
                SetColor("Text", 78, 205, 196);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Green:
                SetColor("Text", 199, 244, 100);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Pink:
                SetColor("Text", 255, 107, 107);
                B2 = new SolidBrush(GetColor("Text"));
                break;
        }
        DrawText(B1, HorizontalAlignment.Center, 1, 1);
        DrawText(B2, HorizontalAlignment.Center, 0, 0);
    }

}
#endregion

#region Label
public partial class genesisLabel : Label /// <The Label has 4 colors defined!>
{
    #region Public Constructors

    MultiColor _color;

    public MultiColor CurrentColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    public genesisLabel()
    {
        this.AutoSize = false;
    }

    #endregion  Public Constructors

    #region  Protected Overridden Methods

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        switch (_color)
        {
            case MultiColor.Black:
                this.ForeColor = Color.FromArgb(24, 24, 24);
                break;
            case MultiColor.Blue:
                this.ForeColor = Color.FromArgb(78, 205, 196);
                break;
            case MultiColor.Green:
                this.ForeColor = Color.FromArgb(199, 244, 100);
                break;
            case MultiColor.Pink:
                this.ForeColor = Color.FromArgb(255, 107, 107);
                break;
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        this.FitToContents();
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);

        this.FitToContents();
    }

    #endregion  Protected Overridden Methods

    #region  Protected Virtual Methods

    protected virtual void FitToContents()
    {
        Size size;

        size = this.GetPreferredSize(new Size(this.Width, 0));

        this.Height = size.Height;
    }

    #endregion  Protected Virtual Methods

    #region  Public Properties

    [DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoSize
    {
        get { return base.AutoSize; }
        set { base.AutoSize = value; }
    }

    #endregion  Public Properties
}
#endregion

#region Draw Class
public class Draw
{
    public static void Gradient(Graphics g, Color c1, Color c2, int x, int y, int width, int height)
    {
        Rectangle R = new Rectangle(x, y, width, height);
        using (LinearGradientBrush T = new LinearGradientBrush(R, c1, c2, LinearGradientMode.Vertical))
        {
            g.FillRectangle(T, R);
        }
    }
    public static void Blend(Graphics g, Color c1, Color c2, Color c3, float c, int d, int x, int y, int width, int height)
    {
        ColorBlend V = new ColorBlend(3);
        V.Colors = new Color[] { c1, c2, c3 };
        V.Positions = new float[] { 0F, c, 1F };
        Rectangle R = new Rectangle(x, y, width, height);
        using (LinearGradientBrush T = new LinearGradientBrush(R, c1, c1, (LinearGradientMode)d))
        {
            T.InterpolationColors = V;
            g.FillRectangle(T, R);
        }
    }
}
#endregion

#region Seperator
internal class genesisSeperator : Control
{

    private Orientation _Orientation;
    public Orientation Orientation
    {
        get
        {
            return _Orientation;
        }
        set
        {
            _Orientation = value;
            UpdateOffset();
            Invalidate();
        }
    }

    private Graphics G;
    private Bitmap B;
    private int I;
    private Color C1;
    private Pen P1;
    private Pen P2;
    public genesisSeperator()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        C1 = Color.FromArgb(250, 250, 250); //Background
        P1 = new Pen(Color.FromArgb(230, 230, 230)); //Shadow
        P2 = new Pen(Color.FromArgb(255, 255, 255)); //Highlight
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        UpdateOffset();
        base.OnSizeChanged(e);
    }

    public void UpdateOffset()
    {
        I = Convert.ToInt32(((_Orientation == 0) ? Height / 2 - 1 : Width / 2 - 1));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        B = new Bitmap(Width, Height);
        G = Graphics.FromImage(B);

        G.Clear(C1);

        if (_Orientation == 0)
        {
            G.DrawLine(P1, 0, I, Width, I);
            G.DrawLine(P2, 0, I + 1, Width, I + 1);
        }
        else
        {
            G.DrawLine(P2, I, 0, I, Height);
            G.DrawLine(P1, I + 1, 0, I + 1, Height);
        }

        e.Graphics.DrawImage(B, 0, 0);
        G.Dispose();
        B.Dispose();
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
    }

}
#endregion

#region Pigment
class Pigment
{
    public string Name { get; set; }
    public Color Value { get; set; }

    public Pigment()
    {
    }

    public Pigment(string n, Color v)
    {
        Name = n;
        Value = v;
    }

    public Pigment(string n, byte a, byte r, byte g, byte b)
    {
        Name = n;
        Value = Color.FromArgb(a, r, g, b);
    }

    public Pigment(string n, byte r, byte g, byte b)
    {
        Name = n;
        Value = Color.FromArgb(r, g, b);
    }
}
#endregion

#region Progress Bar
class genesisProgressBar : ThemeControl154
{
    #region Properties
    private int _Maximum = 100;
    public int Maximum
    {
        get { return _Maximum; }
        set
        {
            if (value < 1)
                value = 1;

            if (value < _Value)
                _Value = value;

            if (_Maximum < _Minimum)
                _Minimum = value;

            _Maximum = value;
            Invalidate();
        }
    }

    private int _Minimum = 0;
    public int Minimum
    {
        get { return _Minimum; }
        set
        {
            if (_Minimum > _Maximum)
                _Maximum = _Minimum;

            if (_Minimum > _Value)
                _Value = _Minimum;

            _Minimum = value;
            Invalidate();
        }
    }

    private int _Value = 0;
    public int Value
    {
        get { return _Value; }
        set
        {
            if (value > _Maximum)
                value = _Maximum;

            if (value < _Minimum)
                value = _Minimum;

            _Value = value;
            Invalidate();
        }
    }
    #endregion
    #region Hatch Options
    HatchColors _color;

    public HatchColors CurrentColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    IsHatched _yesno;

    public IsHatched HatchONOff
    {
        get
        {
            return this._yesno;
        }
        set
        {
            this._yesno = value;
            Invalidate();
        }
    }
    #endregion

    public genesisProgressBar()
    {
        SetColor("Background", Color.FromArgb(245, 245, 245));
        SetColor("Border1", 235, 235, 235);
        SetColor("Border2", 245, 245, 245);
    }

    private Color ProgressBarColor;
    private HatchBrush B1;
    private Pen P1;
    private Pen P2;

    protected override void ColorHook()
    {
        ProgressBarColor = GetColor("Background");
        P1 = new Pen(GetColor("Border1"));
        P2 = new Pen(GetColor("Border2"));
    }

    protected override void PaintHook()
    {
        G.Clear(ProgressBarColor);
        DrawGradient(ProgressBarColor, ProgressBarColor, ClientRectangle, 90f);
        switch (_yesno)
        {
            case IsHatched.Yes:
                switch (_color)
                {
                    case HatchColors.White:
                        SetColor("Hatch1", 245, 245, 245);
                        SetColor("Hatch2", 235, 235, 235);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Blue:
                        SetColor("Hatch1", 78, 205, 196);
                        SetColor("Hatch2", 72, 199, 190);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Green:
                        SetColor("Hatch1", 199, 244, 100);
                        SetColor("Hatch2", 193, 238, 94);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Pink:
                        SetColor("Hatch1", 255, 107, 107);
                        SetColor("Hatch2", 249, 101, 101);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                }
                break;
            case IsHatched.No:
                switch (_color)
                {
                    case HatchColors.White:
                        SetColor("Hatch1", 245, 245, 245);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Blue:
                        SetColor("Hatch1", 78, 205, 196);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Green:
                        SetColor("Hatch1", 199, 244, 100);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Pink:
                        SetColor("Hatch1", 255, 107, 107);
                        SetColor("Hatch2", 249, 101, 101);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                }
                break;
        }
        G.FillRectangle(B1, 2, 2, (Width * _Value) / (_Maximum - _Minimum), Height);
        DrawBorders(P1, 1);
        DrawBorders(P2);
        DrawCorners(BackColor);
    }
    public void Increment(int num)
    {
        this._Value += num;
        Invalidate();
    }
    public void Deincrement(int num)
    {
        this._Value -= num;
        Invalidate();
    }
}
#endregion

#region Checkbox
[DefaultEvent("CheckedChanged")] // This is used to see if it is a checkbox and so on.
class genesisCheckBox : ThemeControl154 // This Checkbox uses a green middle
{
    #region Text Options
    MultiColor _text;

    public MultiColor TextColor
    {
        get
        {
            return this._text;
        }
        set
        {
            this._text = value;
            Invalidate();
        }
    }
    #endregion

    #region Hatch Options
    HatchColors _color;

    public HatchColors CurrentColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    IsHatched _yesno;

    public IsHatched HatchONOff
    {
        get
        {
            return this._yesno;
        }
        set
        {
            this._yesno = value;
            Invalidate();
        }
    }
    #endregion

    public genesisCheckBox()
    {
        Transparent = true; // it is infact transpernt
        BackColor = Color.Transparent;
        // no lockheight since that way they can choose how they want it.
        // no text since i want the user to choose wether or not they want check, also, that way the border doesn't go around it.
        SetColor("Background", 249, 249, 249); // Background is Dark
        SetColor("Border1", 235, 235, 235); // The Inside Border
        SetColor("Border2", 249, 249, 249); // The Outside Border
    }

    // set up the variables
    private Color C1; // Set up Simple Colors
    private Pen P1; // A Pen used to create borders
    private Pen P2;
    private HatchBrush B1;

    protected override void ColorHook()
    {
        C1 = GetColor("Background"); // Get the Colors for the Button Shading
        P1 = new Pen(GetColor("Border1")); // Get and create the borders for the Buttons
        P2 = new Pen(GetColor("Border2"));
    }

    protected override void PaintHook()
    {
        G.Clear(BackColor);
        switch (_Checked)
        {
            case true:
                //Put your checked state here
                switch (_yesno)
                {
                    case IsHatched.Yes:
                        switch (_color)
                        {
                            case HatchColors.White:
                                SetColor("Hatch1", 250, 250, 250);
                                SetColor("Hatch2", 244, 244, 244);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                                break;
                            case HatchColors.Blue:
                                SetColor("Hatch1", 78, 205, 196);
                                SetColor("Hatch2", 72, 199, 190);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                                break;
                            case HatchColors.Green:
                                SetColor("Hatch1", 199, 244, 100);
                                SetColor("Hatch2", 193, 238, 94);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                                break;
                            case HatchColors.Pink:
                                SetColor("Hatch1", 255, 107, 107);
                                SetColor("Hatch2", 249, 101, 101);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                                break;
                        }
                        break;
                    case IsHatched.No:
                        switch (_color)
                        {
                            case HatchColors.White:
                                SetColor("Hatch1", 250, 250, 250);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                                break;
                            case HatchColors.Blue:
                                SetColor("Hatch1", 78, 205, 196);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                                break;
                            case HatchColors.Green:
                                SetColor("Hatch1", 199, 244, 100);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                                break;
                            case HatchColors.Pink:
                                SetColor("Hatch1", 255, 107, 107);
                                SetColor("Hatch2", 249, 101, 101);
                                B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                                break;
                        }
                        break;
                }

                DrawGradient(C1, C1, ClientRectangle, 90f); // checked background
                G.FillRectangle(B1, 3, 3, this.Width - 6, this.Height - 6);
                DrawBorders(P1, 1); // Create the Inner Border
                this.Height = this.Width;
                DrawBorders(P2); // Create the Outer Border
                DrawCorners(BackColor); // Draw the Corners
                break;
            case false:
                //Put your unchecked state here
                DrawGradient(C1, C1, ClientRectangle, 90f); // unchecked background
                DrawBorders(P1, 1); // Create the Inner Border
                this.Height = this.Width;
                DrawBorders(P2); // Create the Outer Border
                DrawCorners(BackColor); // Draw the Corners
                break;
        }
    }

    private bool _Checked { get; set; }
    public bool Checked
    {
        get { return _Checked; }
        set { _Checked = value; }
    }
    protected override void OnClick(System.EventArgs e)
    {
        _Checked = !_Checked;
        if (CheckedChanged != null)
        {
            CheckedChanged(this);
        }
        base.OnClick(e);
    }
    public event CheckedChangedEventHandler CheckedChanged;
    public delegate void CheckedChangedEventHandler(object sender);
}
#endregion

#region MaximizeButton
class genesisMaximize : ThemeControl154 // A Hide Button
{
    HoverOptions _color;

    public HoverOptions HoverColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    public FormWindowState WindowState { get; set; }
    public genesisMaximize()
    {
        SetColor("NoneGradient1", 235, 235, 235);
        SetColor("NoneGradient2", 245, 245, 245);
        SetColor("Text", 24, 24, 24); // The Color for the Text
        SetColor("Texthover", 254, 254, 254); // The Color for the Text
        SetColor("Border1", 225, 225, 225); // The Inside Border
        SetColor("Border2", 235, 235, 235); // The Outside Border
    }

    private Color C1; // Set up Simple Colors
    private Color C2;
    private Color C3;
    private Color C4;
    private Color C5;
    private Color C6;
    private SolidBrush B1; // A Brush to use text
    private SolidBrush B2; // A Brush to use text
    private Pen P1; // A Pen used to create borders
    private Pen P2;

    protected override void ColorHook()
    {
        C3 = GetColor("NoneGradient1");
        C4 = GetColor("NoneGradient2");
        B1 = new SolidBrush(GetColor("Text")); // Set up Color for the Text
        B2 = new SolidBrush(GetColor("Texthover"));
        P1 = new Pen(GetColor("Border1")); // Get and create the borders for the Buttons
        P2 = new Pen(GetColor("Border2"));
    }

    protected override void PaintHook()
    {
        switch (_color)
        {
            case HoverOptions.Blue:
                SetColor("DownGradient1", 72, 199, 190); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 78, 205, 196); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 72, 199, 190);
                SetColor("ClickedGradient2", 78, 205, 196);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Green:
                SetColor("DownGradient1", 193, 238, 94); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 199, 244, 100); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 193, 238, 94);
                SetColor("ClickedGradient2", 199, 244, 100);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Pink:
                SetColor("DownGradient1", 249, 101, 101); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 255, 107, 107); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 249, 101, 101);
                SetColor("ClickedGradient2", 255, 107, 107);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
        }
        if (this.State == MouseState.Over)
        { // Used to see if button is Hovered over
            DrawGradient(C1, C2, ClientRectangle, 90f); // if button is hovered over
            if (Application.OpenForms[0].WindowState == FormWindowState.Normal)
            {
                this.Text = "+";
                DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
            }
            else if (Application.OpenForms[0].WindowState == FormWindowState.Maximized)
            {
                this.Text = "-";
                DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
            }
        }
        else if (this.State == MouseState.Down)
        {
            DrawGradient(C6, C5, ClientRectangle, 90f);
            if (Application.OpenForms[0].WindowState == FormWindowState.Normal)
            {
                this.Text = "+";
                System.Threading.Thread.Sleep(100);
                DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
                Application.OpenForms[0].WindowState = FormWindowState.Maximized;
                this.Text = "-";
            }
            else if (Application.OpenForms[0].WindowState == FormWindowState.Maximized)
            {
                this.Text = "-";
                System.Threading.Thread.Sleep(100);
                DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
                Application.OpenForms[0].WindowState = FormWindowState.Normal;
                this.Text = "+";
            }
        }
        else
        {
            DrawGradient(C3, C4, ClientRectangle, 90f); // else change the shading
            DrawText(B1, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        this.Width = 17;
        this.Height = 17;
        DrawBorders(P1, 1); // Create the Inner Border
        DrawBorders(P2); // Create the Outer Border
        DrawCorners(BackColor); // Draw the Corners
    }

}
#endregion

#region HideButton
class genesisHide : ThemeControl154 // A Hide Button
{
    HoverOptions _color;

    public HoverOptions HoverColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    public FormWindowState WindowState { get; set; }
    public genesisHide()
    {
        SetColor("NoneGradient1", 235, 235, 235);
        SetColor("NoneGradient2", 245, 245, 245);
        SetColor("Text", 24, 24, 24); // The Color for the Text
        SetColor("Texthover", 254, 254, 254); // The Color for the Text
        SetColor("Border1", 225, 225, 225); // The Inside Border
        SetColor("Border2", 235, 235, 235); // The Outside Border
    }

    private Color C1; // Set up Simple Colors
    private Color C2;
    private Color C3;
    private Color C4;
    private Color C5;
    private Color C6;
    private SolidBrush B1; // A Brush to use text
    private SolidBrush B2; // A Brush to use text
    private Pen P1; // A Pen used to create borders
    private Pen P2;

    protected override void ColorHook()
    {
        C3 = GetColor("NoneGradient1");
        C4 = GetColor("NoneGradient2");
        B1 = new SolidBrush(GetColor("Text")); // Set up Color for the Text
        B2 = new SolidBrush(GetColor("Texthover"));
        P1 = new Pen(GetColor("Border1")); // Get and create the borders for the Buttons
        P2 = new Pen(GetColor("Border2"));
    }

    protected override void PaintHook()
    {
        switch (_color)
        {
            case HoverOptions.Blue:
                SetColor("DownGradient1", 72, 199, 190); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 78, 205, 196); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 72, 199, 190);
                SetColor("ClickedGradient2", 78, 205, 196);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Green:
                SetColor("DownGradient1", 193, 238, 94); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 199, 244, 100); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 193, 238, 94);
                SetColor("ClickedGradient2", 199, 244, 100);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Pink:
                SetColor("DownGradient1", 249, 101, 101); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 255, 107, 107); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 249, 101, 101);
                SetColor("ClickedGradient2", 255, 107, 107);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
        }
        if (this.State == MouseState.Over)
        { // Used to see if button is Hovered over
            DrawGradient(C1, C2, ClientRectangle, 90f); // if button is hovered over
            DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
            this.Text = "_";
        }
        else if (this.State == MouseState.Down)
        {
            DrawGradient(C6, C5, ClientRectangle, 90f);
            this.Text = "_";
            DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
            Application.OpenForms[0].WindowState = FormWindowState.Minimized;
        }
        else
        {
            DrawGradient(C3, C4, ClientRectangle, 90f); // else change the shading
            DrawText(B1, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        this.Width = 17;
        this.Height = 17;
        DrawBorders(P1, 1); // Create the Inner Border
        DrawBorders(P2); // Create the Outer Border
        DrawCorners(BackColor); // Draw the Corners
    }

}
#endregion

#region CloseButton
class genesisClose : ThemeControl154 // A Hide Button
{
    HoverOptions _color;

    public HoverOptions HoverColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    public FormWindowState WindowState { get; set; }
    public genesisClose()
    {
        SetColor("NoneGradient1", 235, 235, 235);
        SetColor("NoneGradient2", 245, 245, 245);
        SetColor("Text", 24, 24, 24); // The Color for the Text
        SetColor("Texthover", 254, 254, 254); // The Color for the Text
        SetColor("Border1", 225, 225, 225); // The Inside Border
        SetColor("Border2", 235, 235, 235); // The Outside Border
    }

    private Color C1; // Set up Simple Colors
    private Color C2;
    private Color C3;
    private Color C4;
    private Color C5;
    private Color C6;
    private SolidBrush B1; // A Brush to use text
    private SolidBrush B2; // A Brush to use text
    private Pen P1; // A Pen used to create borders
    private Pen P2;

    protected override void ColorHook()
    {
        C3 = GetColor("NoneGradient1");
        C4 = GetColor("NoneGradient2");
        B1 = new SolidBrush(GetColor("Text")); // Set up Color for the Text
        B2 = new SolidBrush(GetColor("Texthover"));
        P1 = new Pen(GetColor("Border1")); // Get and create the borders for the Buttons
        P2 = new Pen(GetColor("Border2"));
    }

    protected override void PaintHook()
    {
        switch (_color)
        {
            case HoverOptions.Blue:
                SetColor("DownGradient1", 72, 199, 190); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 78, 205, 196); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 72, 199, 190);
                SetColor("ClickedGradient2", 78, 205, 196);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Green:
                SetColor("DownGradient1", 193, 238, 94); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 199, 244, 100); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 193, 238, 94);
                SetColor("ClickedGradient2", 199, 244, 100);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Pink:
                SetColor("DownGradient1", 249, 101, 101); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 255, 107, 107); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 249, 101, 101);
                SetColor("ClickedGradient2", 255, 107, 107);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
        }
        if (this.State == MouseState.Over)
        { // Used to see if button is Hovered over
            DrawGradient(C1, C2, ClientRectangle, 90f); // if button is hovered over
            DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
            this.Text = "x";
        }
        else if (this.State == MouseState.Down)
        {
            DrawGradient(C6, C5, ClientRectangle, 90f);
            this.Text = "x";
            Application.Exit();
            DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        else
        {
            DrawGradient(C3, C4, ClientRectangle, 90f); // else change the shading
            DrawText(B1, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        this.Width = 17;
        this.Height = 17;
        DrawBorders(P1, 1); // Create the Inner Border
        DrawBorders(P2); // Create the Outer Border
        DrawCorners(BackColor); // Draw the Corners
    }

}
#endregion

#region TopButton
class genesisTopButton : ThemeControl154 // A Hide Button
{
    HoverOptions _color;

    public HoverOptions HoverColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }

    public FormWindowState WindowState { get; set; }
    public genesisTopButton()
    {
        SetColor("NoneGradient1", 235, 235, 235);
        SetColor("NoneGradient2", 245, 245, 245);
        SetColor("Text", 24, 24, 24); // The Color for the Text
        SetColor("Texthover", 254, 254, 254); // The Color for the Text
        SetColor("Border1", 225, 225, 225); // The Inside Border
        SetColor("Border2", 235, 235, 235); // The Outside Border
    }

    private Color C1; // Set up Simple Colors
    private Color C2;
    private Color C3;
    private Color C4;
    private Color C5;
    private Color C6;
    private SolidBrush B1; // A Brush to use text
    private SolidBrush B2; // A Brush to use text
    private Pen P1; // A Pen used to create borders
    private Pen P2;

    protected override void ColorHook()
    {
        C3 = GetColor("NoneGradient1");
        C4 = GetColor("NoneGradient2");
        B1 = new SolidBrush(GetColor("Text")); // Set up Color for the Text
        B2 = new SolidBrush(GetColor("Texthover"));
        P1 = new Pen(GetColor("Border1")); // Get and create the borders for the Buttons
        P2 = new Pen(GetColor("Border2"));
    }

    protected override void PaintHook()
    {
        switch (_color)
        {
            case HoverOptions.Blue:
                SetColor("DownGradient1", 72, 199, 190); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 78, 205, 196); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 72, 199, 190);
                SetColor("ClickedGradient2", 78, 205, 196);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Green:
                SetColor("DownGradient1", 193, 238, 94); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 199, 244, 100); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 193, 238, 94);
                SetColor("ClickedGradient2", 199, 244, 100);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
            case HoverOptions.Pink:
                SetColor("DownGradient1", 249, 101, 101); // Basic Gradients Used to Shade the Button
                SetColor("DownGradient2", 255, 107, 107); // The Gradients are reversed, depending on if Button is Pressed or not
                SetColor("ClickedGradient1", 249, 101, 101);
                SetColor("ClickedGradient2", 255, 107, 107);
                C1 = GetColor("DownGradient1"); // Get the Colors for the Button Shading
                C2 = GetColor("DownGradient2");
                C5 = GetColor("ClickedGradient1");
                C6 = GetColor("ClickedGradient2");
                break;
        }
        if (this.State == MouseState.Over)
        { // Used to see if button is Hovered over
            DrawGradient(C1, C2, ClientRectangle, 90f); // if button is hovered over
            DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        else if (this.State == MouseState.Down)
        {
            DrawGradient(C6, C5, ClientRectangle, 90f);
            DrawText(B2, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        else
        {
            DrawGradient(C3, C4, ClientRectangle, 90f); // else change the shading
            DrawText(B1, HorizontalAlignment.Center, 0, 0); // Draw the Text Smack dab in the middle of the button
        }
        this.Width = 17;
        this.Height = 17;
        DrawBorders(P1, 1); // Create the Inner Border
        DrawBorders(P2); // Create the Outer Border
        DrawCorners(BackColor); // Draw the Corners
    }

}
#endregion

#region Groupbox
class genesisGroupBox : ThemeContainer154
{
    #region Text Location
    public enum TextLocation
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    TextLocation _location;

    public TextLocation TextPlacement
    {
        get
        {
            return this._location;
        }
        set
        {
            this._location = value;
            Invalidate();
        }
    }
    #endregion
    #region TextColor
    public enum MultiColor
    {
        Black = 0,
        White = 1,
        Green = 2,
        Blue = 3,
        Pink = 4
    }
    MultiColor _color;

    public MultiColor TextColor
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            Invalidate();
        }
    }
    #endregion
    #region Hatch Options
    HatchColors _hatchcolor;

    public HatchColors HatchColor
    {
        get
        {
            return this._hatchcolor;
        }
        set
        {
            this._hatchcolor = value;
            Invalidate();
        }
    }

    IsHatched _yesno;

    public IsHatched HatchONOff
    {
        get
        {
            return this._yesno;
        }
        set
        {
            this._yesno = value;
            Invalidate();
        }
    }
    #endregion

    public genesisGroupBox()
    {
        ControlMode = true;
        Header = 26;
    }

    private HatchBrush B1;
    private SolidBrush B2;

    protected override void ColorHook()
    {

    }

    protected override void PaintHook()
    {
        switch (_yesno)
        {
            case IsHatched.Yes:
                switch (_hatchcolor)
                {
                    case HatchColors.White:
                        SetColor("Hatch1", 254, 254, 254);
                        SetColor("Hatch2", 248, 248, 248);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Blue:
                        SetColor("Hatch1", 78, 205, 196);
                        SetColor("Hatch2", 72, 199, 190);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Green:
                        SetColor("Hatch1", 199, 244, 100);
                        SetColor("Hatch2", 193, 238, 94);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                    case HatchColors.Pink:
                        SetColor("Hatch1", 255, 107, 107);
                        SetColor("Hatch2", 249, 101, 101);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch2"));
                        break;
                }
                break;
            case IsHatched.No:
                switch (_hatchcolor)
                {
                    case HatchColors.White:
                        SetColor("Hatch1", 254, 254, 254);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Blue:
                        SetColor("Hatch1", 78, 205, 196);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Green:
                        SetColor("Hatch1", 199, 244, 100);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                    case HatchColors.Pink:
                        SetColor("Hatch1", 255, 107, 107);
                        SetColor("Hatch2", 249, 101, 101);
                        B1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, GetColor("Hatch1"), GetColor("Hatch1"));
                        break;
                }
                break;
        }
        G.Clear(Color.FromArgb(249, 249, 249));
        G.FillRectangle(B1, 5, 5, Width - 10, 26);
        G.DrawLine(new Pen(Color.FromArgb(20, Color.White)), 7, 7, Width - 8, 7);
        DrawBorders(new Pen(Color.FromArgb(220, 220, 220)), 5, 5, Width - 10, 26, 1);
        DrawBorders(new Pen(Color.FromArgb(236, 236, 236)), 5, 5, Width - 10, 26);
        DrawBorders(new Pen(Color.FromArgb(60, 208, 208,208)), 5, 34, Width - 10, Height - 39, 1);
        DrawBorders(new Pen(Color.FromArgb(236, 236, 236)), 5, 34, Width - 10, Height - 39);
        DrawBorders(new Pen(Color.FromArgb(236, 236, 236)), 1);
        DrawBorders(new Pen(Color.FromArgb(236, 236, 236)));
        G.DrawLine(new Pen(Color.FromArgb(248, 248, 248)), 1, 1, Width - 2, 1);
        switch (_color)
        {
            case MultiColor.White:
                SetColor("Text", 254, 254, 254);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Black:
                SetColor("Text", 24, 24, 24);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Blue:
                SetColor("Text", 78, 205, 196);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Green:
                SetColor("Text", 199, 244, 100);
                B2 = new SolidBrush(GetColor("Text"));
                break;
            case MultiColor.Pink:
                SetColor("Text", 255, 107, 107);
                B2 = new SolidBrush(GetColor("Text"));
                break;
        }
        switch (_location)
        {
            case TextLocation.Left:
                DrawText(B2, HorizontalAlignment.Left, 9, 5);
                break;
            case TextLocation.Center:
                DrawText(B2, HorizontalAlignment.Center, 0, 5);
                break;
            case TextLocation.Right:
                DrawText(B2, HorizontalAlignment.Right, 10, 5);
                break;
        }

    }
}
#endregion

#region Panel
class genesisPanel : ThemeContainer154
{
    public genesisPanel()
    {
        ControlMode = true;
        Header = 26;
    }

    protected override void ColorHook()
    {

    }

    protected override void PaintHook()
    {
        G.Clear(Color.FromArgb(250, 250, 250));
        DrawBorders(new Pen(Color.FromArgb(236, 236, 236)), 1);
        DrawBorders(new Pen(Color.FromArgb(236, 236, 236)));
        G.DrawLine(new Pen(Color.FromArgb(248, 248, 248)), 1, 1, Width - 2, 1);
    }
}
#endregion

#region Textbox
[DefaultEvent("TextChanged")]
class genesisTextBox : ThemeControl154
{

    private HorizontalAlignment _TextAlign = HorizontalAlignment.Left;
    public HorizontalAlignment TextAlign
    {
        get { return _TextAlign; }
        set
        {
            _TextAlign = value;
            if (Base != null)
            {
                Base.TextAlign = value;
            }
        }
    }
    private int _MaxLength = 32767;
    public int MaxLength
    {
        get { return _MaxLength; }
        set
        {
            _MaxLength = value;
            if (Base != null)
            {
                Base.MaxLength = value;
            }
        }
    }
    private bool _ReadOnly;
    public bool ReadOnly
    {
        get { return _ReadOnly; }
        set
        {
            _ReadOnly = value;
            if (Base != null)
            {
                Base.ReadOnly = value;
            }
        }
    }
    private bool _UseSystemPasswordChar;
    public bool UseSystemPasswordChar
    {
        get { return _UseSystemPasswordChar; }
        set
        {
            _UseSystemPasswordChar = value;
            if (Base != null)
            {
                Base.UseSystemPasswordChar = value;
            }
        }
    }
    private bool _Multiline;
    public bool Multiline
    {
        get { return _Multiline; }
        set
        {
            _Multiline = value;
            if (Base != null)
            {
                Base.Multiline = value;

                if (value)
                {
                    LockHeight = 0;
                    Base.Height = Height - 11;
                }
                else
                {
                    LockHeight = Base.Height + 11;
                }
            }
        }
    }
    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            if (Base != null)
            {
                Base.Text = value;
            }
        }
    }
    public override Font Font
    {
        get { return base.Font; }
        set
        {
            base.Font = value;
            if (Base != null)
            {
                Base.Font = value;
                Base.Location = new Point(3, 5);
                Base.Width = Width - 6;

                if (!_Multiline)
                {
                    LockHeight = Base.Height + 11;
                }
            }
        }
    }

    protected override void OnCreation()
    {
        if (!Controls.Contains(Base))
        {
            Controls.Add(Base);
        }
    }

    private TextBox Base;
    public genesisTextBox()
    {
        Base = new TextBox();

        Base.Font = Font;
        Base.Text = Text;
        Base.MaxLength = _MaxLength;
        Base.Multiline = _Multiline;
        Base.ReadOnly = _ReadOnly;
        Base.UseSystemPasswordChar = _UseSystemPasswordChar;

        Base.BorderStyle = BorderStyle.None;

        Base.Location = new Point(4, 4);
        Base.Width = Width - 10;

        if (_Multiline)
        {
            Base.Height = Height - 11;
        }
        else
        {
            LockHeight = Base.Height + 11;
        }

        Base.TextChanged += OnBaseTextChanged;
        Base.KeyDown += OnBaseKeyDown;


        SetColor("Text", Color.Black);
        SetColor("Backcolor", 245, 245, 245);
        SetColor("Border", 220, 220, 220);
    }

    private Color BG;

    private Pen P1;
    protected override void ColorHook()
    {
        BG = GetColor("Backcolor");

        P1 = GetPen("Border");

        Base.ForeColor = GetColor("Text");
        Base.BackColor = GetColor("Backcolor");
    }

    protected override void PaintHook()
    {
        G.Clear(BG);
        DrawBorders(P1);
    }
    private void OnBaseTextChanged(object s, EventArgs e)
    {
        Text = Base.Text;
    }
    private void OnBaseKeyDown(object s, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            Base.SelectAll();
            e.SuppressKeyPress = true;
        }
    }
    protected override void OnResize(EventArgs e)
    {
        Base.Location = new Point(4, 5);
        Base.Width = Width - 8;

        if (_Multiline)
        {
            Base.Height = Height - 5;
        }


        base.OnResize(e);
    }

}
#endregion

#region RichTextbox
[DefaultEvent("TextChanged")]
class genesisRichTextBox : ThemeControl154
{

    private int _MaxLength = 32767;
    public int MaxLength
    {
        get { return _MaxLength; }
        set
        {
            _MaxLength = value;
            if (Base != null)
            {
                Base.MaxLength = value;
            }
        }
    }
    private bool _ReadOnly;
    public bool ReadOnly
    {
        get { return _ReadOnly; }
        set
        {
            _ReadOnly = value;
            if (Base != null)
            {
                Base.ReadOnly = value;
            }
        }
    }

    private bool _Multiline;
    public bool Multiline
    {
        get { return _Multiline; }
        set
        {
            _Multiline = value;
            if (Base != null)
            {
                Base.Multiline = value;

                if (value)
                {
                    LockHeight = 0;
                    Base.Height = Height - 11;
                }
                else
                {
                    LockHeight = Base.Height + 11;
                }
            }
        }
    }
    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            if (Base != null)
            {
                Base.Text = value;
            }
        }
    }
    public override Font Font
    {
        get { return base.Font; }
        set
        {
            base.Font = value;
            if (Base != null)
            {
                Base.Font = value;
                Base.Location = new Point(3, 5);
                Base.Width = Width - 6;

                if (!_Multiline)
                {
                    LockHeight = Base.Height + 11;
                }
            }
        }
    }

    protected override void OnCreation()
    {
        if (!Controls.Contains(Base))
        {
            Controls.Add(Base);
        }
    }

    private RichTextBox Base;
    public genesisRichTextBox()
    {
        Base = new RichTextBox();

        Base.Font = Font;
        Base.Text = Text;
        Base.MaxLength = _MaxLength;
        Base.Multiline = _Multiline;
        Base.ReadOnly = _ReadOnly;

        Base.BorderStyle = BorderStyle.None;

        Base.Location = new Point(4, 4);
        Base.Width = Width - 10;

        if (_Multiline)
        {
            Base.Height = Height - 11;
        }
        else
        {
            LockHeight = Base.Height + 11;
        }

        Base.TextChanged += OnBaseTextChanged;
        Base.KeyDown += OnBaseKeyDown;


        SetColor("Text", Color.Black);
        SetColor("Backcolor", 245, 245, 245);
        SetColor("Border", 220, 220, 220);
    }

    private Color BG;

    private Pen P1;
    protected override void ColorHook()
    {
        BG = GetColor("Backcolor");

        P1 = GetPen("Border");

        Base.ForeColor = GetColor("Text");
        Base.BackColor = GetColor("Backcolor");
    }

    protected override void PaintHook()
    {
        G.Clear(BG);
        DrawBorders(P1);
    }
    private void OnBaseTextChanged(object s, EventArgs e)
    {
        Text = Base.Text;
    }
    private void OnBaseKeyDown(object s, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            Base.SelectAll();
            e.SuppressKeyPress = true;
        }
    }
    protected override void OnResize(EventArgs e)
    {
        Base.Location = new Point(4, 5);
        Base.Width = Width - 8;

        if (_Multiline)
        {
            Base.Height = Height - 5;
        }


        base.OnResize(e);
    }

}
#endregion

#region Listbox
class genesisListbox : ListBox
{

    public genesisListbox()
    {
        SetStyle(ControlStyles.DoubleBuffer, true);
        Font = new Font("Microsoft Sans Serif", 9);
        BorderStyle = System.Windows.Forms.BorderStyle.None;
        DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        ItemHeight = 21;
        ForeColor = Color.Black;
        BackColor = Color.FromArgb(250, 250, 250);
        IntegralHeight = false;
    }
    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == 15)
            CustomPaint();
    }

    private Image _Image;
    public Image ItemImage
    {
        get { return _Image; }
        set { _Image = value; }
    }

    protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
    {
        try
        {
            if (e.Index < 0)
                return;
            e.DrawBackground();
            Rectangle rect = new Rectangle(new Point(e.Bounds.Left, e.Bounds.Top + 2), new Size(Bounds.Width, 16));
            e.DrawFocusRectangle();
            if (e.State.ToString() == "Selected,")
            {
                Rectangle x2 = e.Bounds;
                Rectangle x3 = new Rectangle(x2.Location, new Size(x2.Width, (x2.Height / 2)));
                LinearGradientBrush G1 = new LinearGradientBrush(new Point(x2.X, x2.Y), new Point(x2.X, x2.Y + x2.Height), Color.FromArgb(250, 250, 250), Color.FromArgb(210, 210, 210));
                HatchBrush H = new HatchBrush(HatchStyle.LightDownwardDiagonal, Color.FromArgb(10, Color.Black), Color.Transparent);
                e.Graphics.FillRectangle(G1, x2);
                G1.Dispose();
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(250, 250, 250)), x3);
                e.Graphics.FillRectangle(H, x2);
                G1.Dispose();
                e.Graphics.DrawString(" " + Items[e.Index].ToString(), Font, Brushes.Black, 5, e.Bounds.Y + (e.Bounds.Height / 2) - 9);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 220)), new Rectangle(new Point(x2.Location.X, x2.Location.Y), new Size(x2.Width, x2.Height)));
            }
            else
            {
                Rectangle x2 = e.Bounds;
                e.Graphics.DrawString(" " + Items[e.Index].ToString(), Font, Brushes.Black, 5, e.Bounds.Y + (e.Bounds.Height / 2) - 9);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(240, 240, 240)), new Rectangle(new Point(x2.Location.X, x2.Location.Y), new Size(x2.Width, x2.Height)));
            }
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 220)), new Rectangle(0, 0, Width - 1, Height - 1));
            base.OnDrawItem(e);
        }
        catch (Exception ex)
        {

        }
    }
    
    public void CustomPaint()
    {
        CreateGraphics().DrawRectangle(new Pen(Color.FromArgb(220, 220, 220)), new Rectangle(0, 0, Width - 1, Height - 1));
    }
}
#endregion
class FlatAlertBox : Control
{

    /// <summary>
    /// How to use: FlatAlertBox.ShowControl(Kind, String, Interval)
    /// </summary>
    /// <remarks></remarks>

    #region " Variables"

    private int W;
    private int H;
    private _Kind K;
    private string _Text;
    private MouseState State = MouseState.None;
    private int X;
    private Timer withEventsField_T;
    private Timer T
    {
        get { return withEventsField_T; }
        set
        {
            if (withEventsField_T != null)
            {
                withEventsField_T.Tick -= T_Tick;
            }
            withEventsField_T = value;
            if (withEventsField_T != null)
            {
                withEventsField_T.Tick += T_Tick;
            }
        }

    }
    #endregion

    #region " Properties"

    [Flags()]
    public enum _Kind
    {
        Success,
        Error,
        Info,
        SkinzDeepBlue,
        GrayDoubt
    }

    #region " Options"


    public _Kind kind
    {
        get { return K; }
        set { K = value; }
    }


    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            if (_Text != null)
            {
                _Text = value;
            }
        }
    }


    public new bool Visible
    {
        get { return base.Visible == false; }
        set { base.Visible = value; }
    }

    #endregion

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Height = 42;
    }

    public void ShowControl(_Kind Kind, string Str, int Interval)
    {
        K = Kind;
        Text = Str;
        this.Visible = true;
        T = new Timer();
        T.Interval = Interval;
        T.Enabled = true;
    }

    private void T_Tick(object sender, EventArgs e)
    {
        this.Visible = false;
        T.Enabled = false;
        T.Dispose();
    }

    #region " Mouse States"

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        State = MouseState.Down;
        Invalidate();
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        State = MouseState.Over;
        Invalidate();
    }
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        State = MouseState.Over;
        Invalidate();
    }
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        State = MouseState.None;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        X = e.X;
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        this.Visible = false;
    }

    #endregion

    #endregion

    #region " Colors"

    private Color SuccessColor = Color.FromArgb(60, 85, 79);
    private Color SuccessText = Color.FromArgb(35, 169, 110);
    private Color ErrorColor = Color.FromArgb(87, 71, 71);
    private Color ErrorText = Color.FromArgb(254, 142, 122);
    private Color InfoColor = Color.FromArgb(70, 91, 94);
    private Color FreakColor = Color.FromArgb(10,105, 178);

    private Color InfoText = Color.FromArgb(97, 185, 186);
    private Color FreakText = Color.FromArgb(10, 153, 255);
    #endregion

    public FlatAlertBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;
        BackColor = Color.FromArgb(60, 70, 73);
        Size = new Size(576, 42);
        Location = new Point(10, 61);
        Font = new Font("Segoe UI", 10);
        Cursor = Cursors.Hand;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var B = new Bitmap(Width, Height);
        var G = Graphics.FromImage(B);
        W = Width - 1;
        H = Height - 1;

        Rectangle Base = new Rectangle(0, 0, W, H);

        var _with14 = G;
        _with14.SmoothingMode = SmoothingMode.HighQuality;
        _with14.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        _with14.Clear(BackColor);

        switch (K)
        {
            case _Kind.Success:
                //-- Base
                _with14.FillRectangle(new SolidBrush(SuccessColor), Base);

                //-- Ellipse
                _with14.FillEllipse(new SolidBrush(SuccessText), new Rectangle(8, 9, 24, 24));
                _with14.FillEllipse(new SolidBrush(SuccessColor), new Rectangle(10, 11, 20, 20));


                //-- Checked Sign



                StringFormat NearSF = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                }; _with14.DrawString("ü", new Font("Wingdings", 22), new SolidBrush(SuccessText), new Rectangle(7, 7, W, H), NearSF);
                _with14.DrawString(Text, Font, new SolidBrush(SuccessText), new Rectangle(48, 12, W, H), NearSF);

                //-- X button
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(W - 30, H - 29, 17, 17));
                _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(SuccessColor), new Rectangle(W - 28, 16, W, H), NearSF);

                switch (State)
                {
                    // -- Mouse Over
                    case MouseState.Over:
                        _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(25, Color.White)), new Rectangle(W - 28, 16, W, H), NearSF);
                        break;
                }

                break;
            case _Kind.Error:
                //-- Base
                _with14.FillRectangle(new SolidBrush(ErrorColor), Base);

                //-- Ellipse
                _with14.FillEllipse(new SolidBrush(ErrorText), new Rectangle(8, 9, 24, 24));
                _with14.FillEllipse(new SolidBrush(ErrorColor), new Rectangle(10, 11, 20, 20));

                //-- X Sign
                NearSF = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                };
                _with14.DrawString("r", new Font("Marlett", 16), new SolidBrush(ErrorText), new Rectangle(6, 11, W, H), NearSF);
                _with14.DrawString(Text, Font, new SolidBrush(ErrorText), new Rectangle(48, 12, W, H), NearSF);

                //-- X button
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(W - 32, H - 29, 17, 17));
                _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(ErrorColor), new Rectangle(W - 30, 17, W, H), NearSF);

                switch (State)
                {
                    case MouseState.Over:
                        // -- Mouse Over
                        _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(25, Color.White)), new Rectangle(W - 30, 15, W, H), NearSF);
                        break;
                }

                break;
            case _Kind.Info:
                //-- Base
                _with14.FillRectangle(new SolidBrush(InfoColor), Base);

                //-- Ellipse
                _with14.FillEllipse(new SolidBrush(InfoText), new Rectangle(8, 9, 24, 24));
                _with14.FillEllipse(new SolidBrush(InfoColor), new Rectangle(10, 11, 20, 20));

                //-- Info Sign
                NearSF = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                };
                _with14.DrawString("¡", new Font("Segoe UI", 20, FontStyle.Bold), new SolidBrush(InfoText), new Rectangle(12, -4, W, H), NearSF);
                _with14.DrawString(Text, Font, new SolidBrush(InfoText), new Rectangle(48, 12, W, H), NearSF);

                //-- X button
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(W - 32, H - 29, 17, 17));
                _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(InfoColor), new Rectangle(W - 30, 17, W, H), NearSF);

                switch (State)
                {
                    case MouseState.Over:
                        // -- Mouse Over
                        _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(25, Color.White)), new Rectangle(W - 30, 17, W, H), NearSF);
                        break;
                }
                break;
            case _Kind.SkinzDeepBlue:
                //-- Base
                _with14.FillRectangle(new SolidBrush(FreakColor), Base);

                //-- Ellipse
               _with14.FillEllipse(new SolidBrush(Color.FromArgb(10, 124, 214)), new Rectangle(8, 9, 24, 24));
               _with14.FillEllipse(new SolidBrush(Color.FromArgb(10, 124, 214)), new Rectangle(10, 11, 20, 20));

                //-- Info Sign
                NearSF = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                };
                _with14.DrawString(">", new Font("Segoe UI",18, FontStyle.Bold), new SolidBrush(FreakColor), new Rectangle(9, 2, W, H), NearSF);
                _with14.DrawString(Text, Font, new SolidBrush(FreakText), new Rectangle(48, 12, W, H), NearSF);

                //-- X button
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(W - 32, H - 29, 17, 17));
                _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(10, 124, 214)), new Rectangle(W - 30, 17, W, H), NearSF);

                switch (State)
                {
                    case MouseState.Over:
                        // -- Mouse Over
                        _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(25, Color.White)), new Rectangle(W - 30, 17, W, H), NearSF);
                        break;
                }
                break;
            case _Kind.GrayDoubt :
                Color GrayDoubtColor = Color.FromArgb(236, 236, 236);
                //-- Base
                _with14.FillRectangle(new SolidBrush(Color.FromArgb(236, 236, 236)), Base);

                //-- Ellipse
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(8, 9, 24, 24));
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(10, 11, 20, 20));

                //-- Info Sign
                NearSF = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                };
           //     _with14.DrawString("¡", new Font("Segoe UI", 20, FontStyle.Bold), new SolidBrush(GrayDoubtColor), new Rectangle(12, -4, W, H), NearSF);
                 _with14.DrawString(">", new Font("Segoe UI", 18, FontStyle.Regular), new SolidBrush(GrayDoubtColor), new Rectangle(9, 2, W, H), NearSF);
                _with14.DrawString(Text, Font, new SolidBrush(Color.FromArgb(80, Color.Black)), new Rectangle(48, 12, W, H), NearSF);

                //-- X button
                _with14.FillEllipse(new SolidBrush(Color.FromArgb(35, Color.Black)), new Rectangle(W - 32, H - 29, 17, 17));
                _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(GrayDoubtColor), new Rectangle(W - 30, 17, W, H), NearSF);

                switch (State)
                {
                    case MouseState.Over:
                        // -- Mouse Over
                        _with14.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(25, Color.White)), new Rectangle(W - 30, 17, W, H), NearSF);
                        break;
                }
                break;
        }


        base.OnPaint(e);
        G.Dispose();
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.DrawImageUnscaled(B, 0, 0);
        B.Dispose();
    }

}