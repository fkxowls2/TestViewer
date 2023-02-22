using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TestViewer
{
    public partial class Form1 : Form
    {
        private delegate void MyFunction();
        private List<MyFunction> functionList = new List<MyFunction>();
        private List<PictureBox> pictureBoxes = new List<PictureBox>();
        private List<Image> imageList = new List<Image>();
        private int listNumber;
        private int timeCount = 0;
        private DateTime loadStartTime;
        private DateTime loadEndTime;
        private DateTime drawStartTime;
        private DateTime drawEndTime;
        private object lockObject = new object();

        private void MakeList()
        {
            pictureBoxes.Add(pictureBox0);
            pictureBoxes.Add(pictureBox1);
            pictureBoxes.Add(pictureBox2);
            pictureBoxes.Add(pictureBox3);
            pictureBoxes.Add(pictureBox4);
            pictureBoxes.Add(pictureBox5);
            pictureBoxes.Add(pictureBox6);
            pictureBoxes.Add(pictureBox7);
            functionList.Add(Function0);
            functionList.Add(Function1);
            functionList.Add(Function2);
            functionList.Add(Function3);
            functionList.Add(Function4);
            functionList.Add(Function5);
            functionList.Add(Function6);
            functionList.Add(Function7);
        }

        public Form1()
        {
            InitializeComponent();
            MakeList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearAll();

            // OpenFileDialog 생성
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // 이미지 파일만 필터링
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            // 멀티 선택 가능
            openFileDialog1.Multiselect = true;

            // 대화 상자가 열리면
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // ListBox에 선택한 파일 경로 추가
                foreach (string fileName in openFileDialog1.FileNames)
                {
                    listBox1.Items.Add(fileName);
                }
            }

            // 불러온 이미지가 8개 보다 많으면 8로 지정
            listNumber = listBox1.Items.Count;
            if (listNumber > 8)
            {
                listNumber = 8;
            }

            // 이미지 로드 스레드 실행
            Thread t = new Thread(new ThreadStart(ImageLoad));
            t.IsBackground = true;
            t.Start();

        }

        private void ImageLoad()
        {
            try
            {
                loadStartTime = DateTime.Now;

                // 이미지 리스트에 이미지 데이터 담기
                for (int i = 0; i < listNumber; i++)
                {
                    imageList.Add(Image.FromFile(listBox1.Items[i].ToString()));
                }

                loadEndTime = DateTime.Now;
                // 이미지 로딩 시간 계산
                TimeSpan duration = loadEndTime - loadStartTime;

                // 이미지 로딩에 대한 정보를 메인 폼에 표시
                Invoke(new Action(() =>
                {
                    listBox2.Items.Add("이미지 로드 완료");
                    listBox2.Items.Add($"이미지 로드 시간: {duration.TotalMilliseconds}ms");
                    button2.Enabled = true;
                }));
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    listBox2.Items.Add("이미지 용량 초과!!!!!");
                }));
            }
        }

        private void ClearAll()
        {
            button2.Enabled = false;
            button3.Enabled = false;
            listBox1.Items.Clear();
            imageList.Clear();
            timeCount = 0;
            ClearPicture();
            GC.Collect();
        }

        private void ClearPicture()
        {
            for (int i = 0; i < 8; i++)
            {
                pictureBoxes[i].Image= null;
                pictureBoxes[i].Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            timeCount = 0;
            ClearPicture();

            drawStartTime = DateTime.Now;
            // 불러온 이미지 개수에 맞춰서 함수 실행
            for (int i = 0; i < listNumber; i++)
            {
                functionList[i]();
            }

            // 이미지 그리기 스레드가 모두 끝나는 지 확인하는 스레드 실행
            Thread tCheckTimeCount = new Thread(new ThreadStart(CheckTimeCount));
            tCheckTimeCount.Start();

        }

        // 이미지 그리기 스레드가 모두 끝나는 지 확인하는 스레드
        private void CheckTimeCount()
        {
            while (true)
            {
                if (timeCount == listNumber)
                {
                    Thread.Sleep(2000);
                    Invoke(new Action(() =>
                    {
                        listBox2.Items.Add("이미지 그리기 완료");
                        button3.Enabled = true;
                    }));
                    break;
                }
            }
        }

        private void Function0()
        {
            Thread t0 = new Thread(new ThreadStart(() => Draw(0)));
            t0.IsBackground = true;
            t0.Start();
        }

        private void Function1()
        {
            Thread t1 = new Thread(new ThreadStart(() => Draw(1)));
            t1.IsBackground = true;
            t1.Start();
        }

        private void Function2()
        {
            Thread t2 = new Thread(new ThreadStart(() => Draw(2)));
            t2.IsBackground = true;
            t2.Start();
        }

        private void Function3()
        {
            Thread t3 = new Thread(new ThreadStart(() => Draw(3)));
            t3.IsBackground = true;
            t3.Start();
        }

        private void Function4()
        {
            Thread t4 = new Thread(new ThreadStart(() => Draw(4)));
            t4.IsBackground = true;
            t4.Start();
        }

        private void Function5()
        {
            Thread t5 = new Thread(new ThreadStart(() => Draw(5)));
            t5.IsBackground = true;
            t5.Start();
        }

        private void Function6()
        {
            Thread t6 = new Thread(new ThreadStart(() => Draw(6)));
            t6.IsBackground = true;
            t6.Start();
        }

        private void Function7()
        {
            Thread t7 = new Thread(new ThreadStart(() => Draw(7)));
            t7.IsBackground = true;
            t7.Start();
        }

        private void Draw(int listNumber)
        {
            Invoke(new Action(() =>
            {
                pictureBoxes[listNumber].Image = imageList[listNumber];
                pictureBoxes[listNumber].SizeMode = PictureBoxSizeMode.Zoom;
                PlusTimeCounter();
            }));
        }

        // 이미지 하나 그릴 때마다 timeCount를 하나씩 증가시키는 함수
        private void PlusTimeCounter()
        {
            lock(lockObject)
            {
                drawEndTime = DateTime.Now;
                timeCount++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 불러온 이미지 개수와 그린 개수가 일치하면 총 그리기 시간 계산 및 표시
            if (timeCount == listNumber)
            {
                button3.Enabled = false;
                TimeSpan duration = drawEndTime - drawStartTime;
                listBox2.Items.Add($"##### 그리기 총 시간: {duration.TotalMilliseconds}ms #####");
                button2.Enabled = true;
            }
            else
            {
                listBox2.Items.Add("아직 Drwaing이 끝나지 않았습니다.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 상태창 지우기
            listBox2.Items.Clear();
        }
    }
}
