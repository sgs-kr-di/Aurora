using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    /// <summary>
    /// Interaction logic for Loading_Window.xaml
    /// </summary>
    public partial class Loading_Window : UserControl
    {
        public Loading_Window()
        {
            InitializeComponent();

            // 애니메이션 설정
            DoubleAnimation dba1 = new DoubleAnimation();  // 애니메이션 생성
            dba1.From = 0;   // start 값
            dba1.To = 360;   // end 값
            dba1.Duration = new Duration(TimeSpan.FromSeconds(1));  // 1.5초 동안 실행
            dba1.RepeatBehavior = RepeatBehavior.Forever;  // 무한 반복

            RotateTransform rt = new RotateTransform();
            imgLoading.RenderTransform = rt;
            rt.CenterX += imgLoading.Width / 2;
            rt.CenterY += imgLoading.Height / 2;

            rt.BeginAnimation(RotateTransform.AngleProperty, dba1);   // 변경할 속성값, 대상애니매이션
        }
    }
}
