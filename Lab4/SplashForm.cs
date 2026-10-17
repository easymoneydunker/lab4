using System.Drawing;
using System.Windows.Forms;

namespace Lab4
{
    public class SplashForm : Form
    {
        public SplashForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(500, 220);
            BackColor = Color.FromArgb(28, 52, 92);
            ShowInTaskbar = false;

            var label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F),
                Text = "Лабораторная работа № 4\n\n" +
                       "Файловые операции, память, буфер обмена\n\n" +
                       "(щёлкните, чтобы продолжить)"
            };
            label.Click += (s, e) => Close();
            Controls.Add(label);

            var timer = new Timer { Interval = 2000 };
            timer.Tick += (s, e) => { timer.Stop(); Close(); };
            Shown += (s, e) => timer.Start();
            FormClosed += (s, e) => timer.Dispose();
        }
    }
}
