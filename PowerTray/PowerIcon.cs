using PowerTray.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerTray
{
    class PowerIcon : IDisposable
    {
        

        private NotifyIcon icon;
        private ContextMenuStrip menu;
        
        private AboutBox about; // the currently open about box

        public PowerIcon()
        {
            icon = new NotifyIcon();
            menu = new ContextMenuStrip();
        }

        /// <summary>
        /// Displays the icon in the system tray.
        /// </summary>
        public void Display()
        {
            icon.MouseClick += new MouseEventHandler(icon_Click);
            icon.Icon = Resources.Icon;
            icon.Text = Resources.IconText;
            icon.Visible = true;

            Refresh();
        }

        /// <summary>
        /// Refreshes the power scheme list
        /// </summary>
        public void Refresh()
        {
            menu.Items.Clear();

            // Get the active scheme
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            Utils.PowerGetActiveScheme(IntPtr.Zero, ptr);
            Guid active = (Guid)Marshal.PtrToStructure((IntPtr)Marshal.PtrToStructure(ptr, typeof(IntPtr)), typeof(Guid));

            // Enumerate schemes
            uint bufferLength = 16;
            byte[] buffer = new byte[bufferLength];
            for (uint i = 0; Utils.PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, Utils.PowerDataAccessor.Scheme, i, buffer, ref bufferLength) == 0; i++)
            {
                Guid id = new Guid(buffer);
                
                /*
                IntPtr idPtr = GCHandle.ToIntPtr(GCHandle.Alloc(id));
                Marshal.StructureToPtr(id, idPtr, true);

                uint var1 = Utils.PowerReadFriendlyName(IntPtr.Zero, idPtr, IntPtr.Zero, IntPtr.Zero, null, ref bufferLength); // get required buffer length
                byte[] nameBuffer = new byte[bufferLength];
                uint var2 = Utils.PowerReadFriendlyName(IntPtr.Zero, idPtr, IntPtr.Zero, IntPtr.Zero, buffer, ref bufferLength); // read into buffer for real
                 */

                //ToolStripMenuItem item = new ToolStripMenuItem(Encoding.Unicode.GetString(nameBuffer));
                ToolStripMenuItem item = new ToolStripMenuItem(id.ToString());
                item.Click += new EventHandler(Scheme_Click);
                item.Checked = id.Equals(active); // mark if active
                
                /*
                 * (scheme index -> shortcut key) map
                 * 0 -> D0
                 * ...
                 * 9 -> D9
                 * 10 -> A
                 * ...
                 * 36 -> Z
                 * else -> none
                 */
                if (i > Keys.Z - Keys.D0 - (Keys.A - Keys.D9)) item.ShortcutKeys = (Keys)((int)Keys.D0 + i + (i > 9 ? Keys.A - Keys.D9 : 0));
                item.Tag = id;
                menu.Items.Add(item);
            }

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(new ToolStripMenuItem(Resources.RefreshText, Resources.RefreshIcon, Refresh_Click, Keys.F5));
            menu.Items.Add(new ToolStripMenuItem(Resources.AboutText, Resources.AboutIcon, About_Click, Keys.F1));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            icon.Dispose();
            menu.Dispose();
            about.Dispose();
        }

        /// <summary>
        /// Handles notify icon click
        /// </summary>
        void icon_Click(object sender, EventArgs e)
        {
            menu.Show(Cursor.Position);
        }

        /// <summary>
        /// Handles scheme menu items click
        /// </summary>
        void Scheme_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                Guid id = (Guid)((ToolStripItem)sender).Tag;

                IntPtr idPtr = GCHandle.ToIntPtr(GCHandle.Alloc(id));
                Marshal.StructureToPtr(id, idPtr, true);

                Utils.PowerSetActiveScheme(IntPtr.Zero, idPtr);
            }
            Refresh();
        }

        /// <summary>
        /// Handles refresh menu item click
        /// </summary>
        void Refresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Handles about menu item click
        /// </summary>
        void About_Click(object sender, EventArgs e)
        {
            if (about == null)
            {
                about = new AboutBox();
                about.ShowDialog();
                about = null;
            }
            else
            {
                if (about.WindowState == FormWindowState.Minimized) about.WindowState = FormWindowState.Normal; // restore if minimized
                about.Activate(); // bring to front
            }
        }
    }
}
