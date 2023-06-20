﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms;

/// <summary>
///  Decodes Windows messages. This is in a separate class from Message so we can avoid
///  loading it in the 99% case where we don't need it.
/// </summary>
internal static class MessageDecoder
{
    /// <summary>
    ///  Returns the symbolic name of the msg value, or null if it isn't one of the
    ///  existing constants.
    /// </summary>
    private static string? MsgToString(User32.WM msg)
    {
        string? text = msg switch
        {
            User32.WM.NULL => "WM_NULL",
            User32.WM.CREATE => "WM_CREATE",
            User32.WM.DESTROY => "WM_DESTROY",
            User32.WM.MOVE => "WM_MOVE",
            User32.WM.SIZE => "WM_SIZE",
            User32.WM.ACTIVATE => "WM_ACTIVATE",
            User32.WM.SETFOCUS => "WM_SETFOCUS",
            User32.WM.KILLFOCUS => "WM_KILLFOCUS",
            User32.WM.ENABLE => "WM_ENABLE",
            User32.WM.SETREDRAW => "WM_SETREDRAW",
            User32.WM.SETTEXT => "WM_SETTEXT",
            User32.WM.GETTEXT => "WM_GETTEXT",
            User32.WM.GETTEXTLENGTH => "WM_GETTEXTLENGTH",
            User32.WM.PAINT => "WM_PAINT",
            User32.WM.CLOSE => "WM_CLOSE",
            User32.WM.QUERYENDSESSION => "WM_QUERYENDSESSION",
            User32.WM.QUIT => "WM_QUIT",
            User32.WM.QUERYOPEN => "WM_QUERYOPEN",
            User32.WM.ERASEBKGND => "WM_ERASEBKGND",
            User32.WM.SYSCOLORCHANGE => "WM_SYSCOLORCHANGE",
            User32.WM.ENDSESSION => "WM_ENDSESSION",
            User32.WM.SHOWWINDOW => "WM_SHOWWINDOW",
            User32.WM.WININICHANGE => "WM_WININICHANGE",
            User32.WM.DEVMODECHANGE => "WM_DEVMODECHANGE",
            User32.WM.ACTIVATEAPP => "WM_ACTIVATEAPP",
            User32.WM.FONTCHANGE => "WM_FONTCHANGE",
            User32.WM.TIMECHANGE => "WM_TIMECHANGE",
            User32.WM.CANCELMODE => "WM_CANCELMODE",
            User32.WM.SETCURSOR => "WM_SETCURSOR",
            User32.WM.MOUSEACTIVATE => "WM_MOUSEACTIVATE",
            User32.WM.CHILDACTIVATE => "WM_CHILDACTIVATE",
            User32.WM.QUEUESYNC => "WM_QUEUESYNC",
            User32.WM.GETMINMAXINFO => "WM_GETMINMAXINFO",
            User32.WM.PAINTICON => "WM_PAINTICON",
            User32.WM.ICONERASEBKGND => "WM_ICONERASEBKGND",
            User32.WM.NEXTDLGCTL => "WM_NEXTDLGCTL",
            User32.WM.SPOOLERSTATUS => "WM_SPOOLERSTATUS",
            User32.WM.DRAWITEM => "WM_DRAWITEM",
            User32.WM.MEASUREITEM => "WM_MEASUREITEM",
            User32.WM.DELETEITEM => "WM_DELETEITEM",
            User32.WM.VKEYTOITEM => "WM_VKEYTOITEM",
            User32.WM.CHARTOITEM => "WM_CHARTOITEM",
            User32.WM.SETFONT => "WM_SETFONT",
            User32.WM.GETFONT => "WM_GETFONT",
            User32.WM.SETHOTKEY => "WM_SETHOTKEY",
            User32.WM.GETHOTKEY => "WM_GETHOTKEY",
            User32.WM.QUERYDRAGICON => "WM_QUERYDRAGICON",
            User32.WM.COMPAREITEM => "WM_COMPAREITEM",
            User32.WM.GETOBJECT => "WM_GETOBJECT",
            User32.WM.COMPACTING => "WM_COMPACTING",
            User32.WM.COMMNOTIFY => "WM_COMMNOTIFY",
            User32.WM.WINDOWPOSCHANGING => "WM_WINDOWPOSCHANGING",
            User32.WM.WINDOWPOSCHANGED => "WM_WINDOWPOSCHANGED",
            User32.WM.POWER => "WM_POWER",
            User32.WM.COPYDATA => "WM_COPYDATA",
            User32.WM.CANCELJOURNAL => "WM_CANCELJOURNAL",
            User32.WM.NOTIFY => "WM_NOTIFY",
            User32.WM.INPUTLANGCHANGEREQUEST => "WM_INPUTLANGCHANGEREQUEST",
            User32.WM.INPUTLANGCHANGE => "WM_INPUTLANGCHANGE",
            User32.WM.TCARD => "WM_TCARD",
            User32.WM.HELP => "WM_HELP",
            User32.WM.USERCHANGED => "WM_USERCHANGED",
            User32.WM.NOTIFYFORMAT => "WM_NOTIFYFORMAT",
            User32.WM.CONTEXTMENU => "WM_CONTEXTMENU",
            User32.WM.STYLECHANGING => "WM_STYLECHANGING",
            User32.WM.STYLECHANGED => "WM_STYLECHANGED",
            User32.WM.DISPLAYCHANGE => "WM_DISPLAYCHANGE",
            User32.WM.GETICON => "WM_GETICON",
            User32.WM.SETICON => "WM_SETICON",
            User32.WM.NCCREATE => "WM_NCCREATE",
            User32.WM.NCDESTROY => "WM_NCDESTROY",
            User32.WM.NCCALCSIZE => "WM_NCCALCSIZE",
            User32.WM.NCHITTEST => "WM_NCHITTEST",
            User32.WM.NCPAINT => "WM_NCPAINT",
            User32.WM.NCACTIVATE => "WM_NCACTIVATE",
            User32.WM.GETDLGCODE => "WM_GETDLGCODE",
            User32.WM.NCMOUSEMOVE => "WM_NCMOUSEMOVE",
            User32.WM.NCLBUTTONDOWN => "WM_NCLBUTTONDOWN",
            User32.WM.NCLBUTTONUP => "WM_NCLBUTTONUP",
            User32.WM.NCLBUTTONDBLCLK => "WM_NCLBUTTONDBLCLK",
            User32.WM.NCRBUTTONDOWN => "WM_NCRBUTTONDOWN",
            User32.WM.NCRBUTTONUP => "WM_NCRBUTTONUP",
            User32.WM.NCRBUTTONDBLCLK => "WM_NCRBUTTONDBLCLK",
            User32.WM.NCMBUTTONDOWN => "WM_NCMBUTTONDOWN",
            User32.WM.NCMBUTTONUP => "WM_NCMBUTTONUP",
            User32.WM.NCMBUTTONDBLCLK => "WM_NCMBUTTONDBLCLK",
            User32.WM.KEYDOWN => "WM_KEYDOWN",
            User32.WM.KEYUP => "WM_KEYUP",
            User32.WM.CHAR => "WM_CHAR",
            User32.WM.DEADCHAR => "WM_DEADCHAR",
            User32.WM.SYSKEYDOWN => "WM_SYSKEYDOWN",
            User32.WM.SYSKEYUP => "WM_SYSKEYUP",
            User32.WM.SYSCHAR => "WM_SYSCHAR",
            User32.WM.SYSDEADCHAR => "WM_SYSDEADCHAR",
            User32.WM.KEYLAST => "WM_KEYLAST",
            User32.WM.IME_STARTCOMPOSITION => "WM_IME_STARTCOMPOSITION",
            User32.WM.IME_ENDCOMPOSITION => "WM_IME_ENDCOMPOSITION",
            User32.WM.IME_COMPOSITION => "WM_IME_COMPOSITION",
            User32.WM.INITDIALOG => "WM_INITDIALOG",
            User32.WM.COMMAND => "WM_COMMAND",
            User32.WM.SYSCOMMAND => "WM_SYSCOMMAND",
            User32.WM.TIMER => "WM_TIMER",
            User32.WM.HSCROLL => "WM_HSCROLL",
            User32.WM.VSCROLL => "WM_VSCROLL",
            User32.WM.INITMENU => "WM_INITMENU",
            User32.WM.INITMENUPOPUP => "WM_INITMENUPOPUP",
            User32.WM.MENUSELECT => "WM_MENUSELECT",
            User32.WM.MENUCHAR => "WM_MENUCHAR",
            User32.WM.ENTERIDLE => "WM_ENTERIDLE",
            User32.WM.CTLCOLORMSGBOX => "WM_CTLCOLORMSGBOX",
            User32.WM.CTLCOLOREDIT => "WM_CTLCOLOREDIT",
            User32.WM.CTLCOLORLISTBOX => "WM_CTLCOLORLISTBOX",
            User32.WM.CTLCOLORBTN => "WM_CTLCOLORBTN",
            User32.WM.CTLCOLORDLG => "WM_CTLCOLORDLG",
            User32.WM.CTLCOLORSCROLLBAR => "WM_CTLCOLORSCROLLBAR",
            User32.WM.CTLCOLORSTATIC => "WM_CTLCOLORSTATIC",
            User32.WM.MOUSEMOVE => "WM_MOUSEMOVE",
            User32.WM.LBUTTONDOWN => "WM_LBUTTONDOWN",
            User32.WM.LBUTTONUP => "WM_LBUTTONUP",
            User32.WM.LBUTTONDBLCLK => "WM_LBUTTONDBLCLK",
            User32.WM.RBUTTONDOWN => "WM_RBUTTONDOWN",
            User32.WM.RBUTTONUP => "WM_RBUTTONUP",
            User32.WM.RBUTTONDBLCLK => "WM_RBUTTONDBLCLK",
            User32.WM.MBUTTONDOWN => "WM_MBUTTONDOWN",
            User32.WM.MBUTTONUP => "WM_MBUTTONUP",
            User32.WM.MBUTTONDBLCLK => "WM_MBUTTONDBLCLK",
            User32.WM.MOUSEWHEEL => "WM_MOUSEWHEEL",
            User32.WM.PARENTNOTIFY => "WM_PARENTNOTIFY",
            User32.WM.ENTERMENULOOP => "WM_ENTERMENULOOP",
            User32.WM.EXITMENULOOP => "WM_EXITMENULOOP",
            User32.WM.NEXTMENU => "WM_NEXTMENU",
            User32.WM.SIZING => "WM_SIZING",
            User32.WM.CAPTURECHANGED => "WM_CAPTURECHANGED",
            User32.WM.MOVING => "WM_MOVING",
            User32.WM.POWERBROADCAST => "WM_POWERBROADCAST",
            User32.WM.DEVICECHANGE => "WM_DEVICECHANGE",
            User32.WM.IME_SETCONTEXT => "WM_IME_SETCONTEXT",
            User32.WM.IME_NOTIFY => "WM_IME_NOTIFY",
            User32.WM.IME_CONTROL => "WM_IME_CONTROL",
            User32.WM.IME_COMPOSITIONFULL => "WM_IME_COMPOSITIONFULL",
            User32.WM.IME_SELECT => "WM_IME_SELECT",
            User32.WM.IME_CHAR => "WM_IME_CHAR",
            User32.WM.IME_KEYDOWN => "WM_IME_KEYDOWN",
            User32.WM.IME_KEYUP => "WM_IME_KEYUP",
            User32.WM.MDICREATE => "WM_MDICREATE",
            User32.WM.MDIDESTROY => "WM_MDIDESTROY",
            User32.WM.MDIACTIVATE => "WM_MDIACTIVATE",
            User32.WM.MDIRESTORE => "WM_MDIRESTORE",
            User32.WM.MDINEXT => "WM_MDINEXT",
            User32.WM.MDIMAXIMIZE => "WM_MDIMAXIMIZE",
            User32.WM.MDITILE => "WM_MDITILE",
            User32.WM.MDICASCADE => "WM_MDICASCADE",
            User32.WM.MDIICONARRANGE => "WM_MDIICONARRANGE",
            User32.WM.MDIGETACTIVE => "WM_MDIGETACTIVE",
            User32.WM.MDISETMENU => "WM_MDISETMENU",
            User32.WM.ENTERSIZEMOVE => "WM_ENTERSIZEMOVE",
            User32.WM.EXITSIZEMOVE => "WM_EXITSIZEMOVE",
            User32.WM.DROPFILES => "WM_DROPFILES",
            User32.WM.MDIREFRESHMENU => "WM_MDIREFRESHMENU",
            User32.WM.MOUSEHOVER => "WM_MOUSEHOVER",
            User32.WM.MOUSELEAVE => "WM_MOUSELEAVE",
            User32.WM.CUT => "WM_CUT",
            User32.WM.COPY => "WM_COPY",
            User32.WM.PASTE => "WM_PASTE",
            User32.WM.CLEAR => "WM_CLEAR",
            User32.WM.UNDO => "WM_UNDO",
            User32.WM.RENDERFORMAT => "WM_RENDERFORMAT",
            User32.WM.RENDERALLFORMATS => "WM_RENDERALLFORMATS",
            User32.WM.DESTROYCLIPBOARD => "WM_DESTROYCLIPBOARD",
            User32.WM.DRAWCLIPBOARD => "WM_DRAWCLIPBOARD",
            User32.WM.PAINTCLIPBOARD => "WM_PAINTCLIPBOARD",
            User32.WM.VSCROLLCLIPBOARD => "WM_VSCROLLCLIPBOARD",
            User32.WM.SIZECLIPBOARD => "WM_SIZECLIPBOARD",
            User32.WM.ASKCBFORMATNAME => "WM_ASKCBFORMATNAME",
            User32.WM.CHANGECBCHAIN => "WM_CHANGECBCHAIN",
            User32.WM.HSCROLLCLIPBOARD => "WM_HSCROLLCLIPBOARD",
            User32.WM.QUERYNEWPALETTE => "WM_QUERYNEWPALETTE",
            User32.WM.PALETTEISCHANGING => "WM_PALETTEISCHANGING",
            User32.WM.PALETTECHANGED => "WM_PALETTECHANGED",
            User32.WM.HOTKEY => "WM_HOTKEY",
            User32.WM.PRINT => "WM_PRINT",
            User32.WM.PRINTCLIENT => "WM_PRINTCLIENT",
            User32.WM.HANDHELDFIRST => "WM_HANDHELDFIRST",
            User32.WM.HANDHELDLAST => "WM_HANDHELDLAST",
            User32.WM.AFXFIRST => "WM_AFXFIRST",
            User32.WM.AFXLAST => "WM_AFXLAST",
            User32.WM.PENWINFIRST => "WM_PENWINFIRST",
            User32.WM.PENWINLAST => "WM_PENWINLAST",
            User32.WM.APP => "WM_APP",
            User32.WM.USER => "WM_USER",
            User32.WM.CTLCOLOR => "WM_CTLCOLOR",

            // RichEdit messages
            (User32.WM)PInvoke.EM_GETLIMITTEXT => "EM_GETLIMITTEXT",
            (User32.WM)PInvoke.EM_POSFROMCHAR => "EM_POSFROMCHAR",
            (User32.WM)PInvoke.EM_CHARFROMPOS => "EM_CHARFROMPOS",
            (User32.WM)PInvoke.EM_SCROLLCARET => "EM_SCROLLCARET",
            (User32.WM)Richedit.EM.CANPASTE => "EM_CANPASTE",
            (User32.WM)Richedit.EM.DISPLAYBAND => "EM_DISPLAYBAND",
            (User32.WM)Richedit.EM.EXGETSEL => "EM_EXGETSEL",
            (User32.WM)Richedit.EM.EXLIMITTEXT => "EM_EXLIMITTEXT",
            (User32.WM)Richedit.EM.EXLINEFROMCHAR => "EM_EXLINEFROMCHAR",
            (User32.WM)Richedit.EM.EXSETSEL => "EM_EXSETSEL",
            (User32.WM)Richedit.EM.FINDTEXT => "EM_FINDTEXT",
            (User32.WM)Richedit.EM.FORMATRANGE => "EM_FORMATRANGE",
            (User32.WM)Richedit.EM.GETCHARFORMAT => "EM_GETCHARFORMAT",
            (User32.WM)Richedit.EM.GETEVENTMASK => "EM_GETEVENTMASK",
            (User32.WM)Richedit.EM.GETOLEINTERFACE => "EM_GETOLEINTERFACE",
            (User32.WM)Richedit.EM.GETPARAFORMAT => "EM_GETPARAFORMAT",
            (User32.WM)Richedit.EM.GETSELTEXT => "EM_GETSELTEXT",
            (User32.WM)Richedit.EM.HIDESELECTION => "EM_HIDESELECTION",
            (User32.WM)Richedit.EM.PASTESPECIAL => "EM_PASTESPECIAL",
            (User32.WM)Richedit.EM.REQUESTRESIZE => "EM_REQUESTRESIZE",
            (User32.WM)Richedit.EM.SELECTIONTYPE => "EM_SELECTIONTYPE",
            (User32.WM)Richedit.EM.SETBKGNDCOLOR => "EM_SETBKGNDCOLOR",
            (User32.WM)Richedit.EM.SETCHARFORMAT => "EM_SETCHARFORMAT",
            (User32.WM)Richedit.EM.SETEVENTMASK => "EM_SETEVENTMASK",
            (User32.WM)Richedit.EM.SETOLECALLBACK => "EM_SETOLECALLBACK",
            (User32.WM)Richedit.EM.SETPARAFORMAT => "EM_SETPARAFORMAT",
            (User32.WM)Richedit.EM.SETTARGETDEVICE => "EM_SETTARGETDEVICE",
            (User32.WM)Richedit.EM.STREAMIN => "EM_STREAMIN",
            (User32.WM)Richedit.EM.STREAMOUT => "EM_STREAMOUT",
            (User32.WM)Richedit.EM.GETTEXTRANGE => "EM_GETTEXTRANGE",
            (User32.WM)Richedit.EM.FINDWORDBREAK => "EM_FINDWORDBREAK",
            (User32.WM)Richedit.EM.SETOPTIONS => "EM_SETOPTIONS",
            (User32.WM)Richedit.EM.GETOPTIONS => "EM_GETOPTIONS",
            (User32.WM)Richedit.EM.FINDTEXTEX => "EM_FINDTEXTEX",
            (User32.WM)Richedit.EM.GETWORDBREAKPROCEX => "EM_GETWORDBREAKPROCEX",
            (User32.WM)Richedit.EM.SETWORDBREAKPROCEX => "EM_SETWORDBREAKPROCEX",

            // Richedit v2.0 messages
            (User32.WM)Richedit.EM.SETUNDOLIMIT => "EM_SETUNDOLIMIT",
            (User32.WM)Richedit.EM.REDO => "EM_REDO",
            (User32.WM)Richedit.EM.CANREDO => "EM_CANREDO",
            (User32.WM)Richedit.EM.GETUNDONAME => "EM_GETUNDONAME",
            (User32.WM)Richedit.EM.GETREDONAME => "EM_GETREDONAME",
            (User32.WM)Richedit.EM.STOPGROUPTYPING => "EM_STOPGROUPTYPING",
            (User32.WM)Richedit.EM.SETTEXTMODE => "EM_SETTEXTMODE",
            (User32.WM)Richedit.EM.GETTEXTMODE => "EM_GETTEXTMODE",
            (User32.WM)Richedit.EM.AUTOURLDETECT => "EM_AUTOURLDETECT",
            (User32.WM)Richedit.EM.GETAUTOURLDETECT => "EM_GETAUTOURLDETECT",
            (User32.WM)Richedit.EM.SETPALETTE => "EM_SETPALETTE",
            (User32.WM)Richedit.EM.GETTEXTEX => "EM_GETTEXTEX",
            (User32.WM)Richedit.EM.GETTEXTLENGTHEX => "EM_GETTEXTLENGTHEX",

            // Asia specific messages
            (User32.WM)Richedit.EM.SETPUNCTUATION => "EM_SETPUNCTUATION",
            (User32.WM)Richedit.EM.GETPUNCTUATION => "EM_GETPUNCTUATION",
            (User32.WM)Richedit.EM.SETWORDWRAPMODE => "EM_SETWORDWRAPMODE",
            (User32.WM)Richedit.EM.GETWORDWRAPMODE => "EM_GETWORDWRAPMODE",
            (User32.WM)Richedit.EM.SETIMECOLOR => "EM_SETIMECOLOR",
            (User32.WM)Richedit.EM.GETIMECOLOR => "EM_GETIMECOLOR",
            (User32.WM)Richedit.EM.SETIMEOPTIONS => "EM_SETIMEOPTIONS",
            (User32.WM)Richedit.EM.GETIMEOPTIONS => "EM_GETIMEOPTIONS",
            (User32.WM)Richedit.EM.CONVPOSITION => "EM_CONVPOSITION",
            (User32.WM)Richedit.EM.SETLANGOPTIONS => "EM_SETLANGOPTIONS",
            (User32.WM)Richedit.EM.GETLANGOPTIONS => "EM_GETLANGOPTIONS",
            (User32.WM)Richedit.EM.GETIMECOMPMODE => "EM_GETIMECOMPMODE",
            (User32.WM)Richedit.EM.FINDTEXTW => "EM_FINDTEXTW",
            (User32.WM)Richedit.EM.FINDTEXTEXW => "EM_FINDTEXTEXW",

            // Rich Edit 3.0 Asia msgs
            (User32.WM)Richedit.EM.RECONVERSION => "EM_RECONVERSION",
            (User32.WM)Richedit.EM.SETIMEMODEBIAS => "EM_SETIMEMODEBIAS",
            (User32.WM)Richedit.EM.GETIMEMODEBIAS => "EM_GETIMEMODEBIAS",

            // BiDi Specific messages
            (User32.WM)Richedit.EM.SETBIDIOPTIONS => "EM_SETBIDIOPTIONS",
            (User32.WM)Richedit.EM.GETBIDIOPTIONS => "EM_GETBIDIOPTIONS",
            (User32.WM)Richedit.EM.SETTYPOGRAPHYOPTIONS => "EM_SETTYPOGRAPHYOPTIONS",
            (User32.WM)Richedit.EM.GETTYPOGRAPHYOPTIONS => "EM_GETTYPOGRAPHYOPTIONS",

            // Extended Edit style specific messages
            (User32.WM)Richedit.EM.SETEDITSTYLE => "EM_SETEDITSTYLE",
            (User32.WM)Richedit.EM.GETEDITSTYLE => "EM_GETEDITSTYLE",
            _ => null,
        };

        if (text is null && ((msg & User32.WM.REFLECT) == User32.WM.REFLECT))
        {
            string subtext = MsgToString((User32.WM)(msg - User32.WM.REFLECT)) ?? "???";

            text = $"WM_REFLECT + {subtext}";
        }

        return text;
    }

    public static string ToString(Message message)
    {
        return ToString(
            message.HWnd,
            message.MsgInternal,
            message.WParamInternal,
            message.LParamInternal,
            message.ResultInternal);
    }

    private static string ToString(IntPtr hWnd, User32.WM msg, WPARAM wparam, LPARAM lparam, LRESULT result)
    {
        static string Parenthesize(string? input) => input is null ? string.Empty : $" ({input})";

        string id = Parenthesize(MsgToString(msg));

        string lDescription = string.Empty;
        if (msg == User32.WM.PARENTNOTIFY)
        {
            lDescription = Parenthesize(MsgToString((User32.WM)wparam.LOWORD));
        }

        return $@"msg=0x{(uint)msg:x}{id} hwnd=0x{(long)hWnd:x} wparam=0x{(nint)wparam:x} lparam=0x{(nint)lparam:x}{lDescription} result=0x{(nint)result:x}";
    }
}
