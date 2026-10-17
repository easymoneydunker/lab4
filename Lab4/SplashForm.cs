using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab4
{
    /// <summary>
    /// Заставка, показываемая при запуске приложения.
    /// Закрывается автоматически через несколько секунд или по щелчку мыши / нажатию клавиши.
    /// </summary>
    public class SplashForm : Form
    {
        private const int ShowTimeMs = 2500;

        private readonly Timer timer;
        private readonly ProgressBar progress;
        private int elapsed;

        public SplashForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(560, 300);
            BackColor = Color.FromArgb(28, 52, 92);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);
            KeyPreview = true;

            var lblTitle = new Label
            {
                Text = "Лабораторная работа № 4",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 40, 560, 50)
            };
            var lblSubtitle = new Label
            {
                Text = "Создание приложений Windows с использованием\nсредств прикладного программирования Win32 API",
                Font = new Font("Segoe UI", 11F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 95, 560, 50)
            };
            var lblParts = new Label
            {
                Text = "Файловые операции  •  Память  •  Буфер обмена",
                ForeColor = Color.FromArgb(170, 200, 240),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 160, 560, 25)
            };
            progress = new ProgressBar
            {
                Bounds = new Rectangle(60, 215, 440, 14),
                Maximum = ShowTimeMs,
                Style = ProgressBarStyle.Continuous
            };
            var lblHint = new Label
            {
                Text = "Загрузка... (щёлкните, чтобы пропустить)",
                ForeColor = Color.FromArgb(170, 200, 240),
                Font = new Font("Segoe UI", 8.5F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 240, 560, 20)
            };

            Controls.AddRange(new Control[] { lblTitle, lblSubtitle, lblParts, progress, lblHint });
            foreach (Control c in Controls)
                c.Click += (s, e) => Close();
            Click += (s, e) => Close();
            KeyDown += (s, e) => Close();

            timer = new Timer { Interval = 50 };
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                progress.Value = Math.Min(elapsed, ShowTimeMs);
                if (elapsed >= ShowTimeMs)
                    Close();
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(Color.FromArgb(90, 130, 190), 2))
                e.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timer.Stop();
            base.OnFormClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                timer.Dispose();
            base.Dispose(disposing);
        }
    }
}
