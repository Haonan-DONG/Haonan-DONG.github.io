// lack
//CR and CI analysis.
// Register part codes.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

// Port transfer module.
using SerialPortConnection;

// Data initial module.
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Diagnostics;

// Algorithm module.
using LinearAlgebra;   // Linera Algebra Class.
using LinearAlgebra.MatrixAlgebra;
using System.Runtime.InteropServices;

// TAO.OPENGL for monitor module.
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;


namespace Evacuation_Routine_Planning_System
{
    public partial class E_R_P_Interface : Form
    {
        // ----------------------------Global Initialize----------------------------
        Global.Mapping map;
        Global.Sequence sequence;
        AutoResetEvent monitorAHPThread = new AutoResetEvent(false);
        public Matrix R;

        // Declared static (no need for the 
        static float X = 0.0f;        // Translate screen to x direction (left or right)
        static float Y = 0.0f;        // Translate screen to y direction (up or down)
        static float Z = 0.0f;        // Translate screen to z direction (zoom in or out)
        static float rotX = 0.0f;    // Rotate screen on x axis 
        static float rotY = 0.0f;    // Rotate screen on y axis
        static float rotZ = 0.0f;    // Rotate screen on z axis

        static float rotLx = 0.0f;   // Translate screen by using the glulookAt function 
                                     // (left or right)
        static float rotLy = 0.0f;   // Translate screen by using the glulookAt function 
                                     // (up or down)
        static float rotLz = 0.0f;   // Translate screen by using the glulookAt function 
                                     // (zoom in or out)

        static bool lines = true;       // Display x,y,z lines (coordinate lines)
        static bool rotation = false;   // Rotate if F2 is pressed   
        static int old_x, old_y;        // Used for mouse event
        static int mousePressed;

        // ---------------------------- Interface Inititalize ------------------------
        public E_R_P_Interface()
        {
            InitializeComponent();
        }


        // -------------------------------- Port and File Module -----------------------------------------------------
        // and this should be a new thread with lots of work to do.
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form portForm = new Port_Form();
            portForm.Show();   
        }


        // --------------------------- Data Initialized Module ------------------------
        // open file to read map data.
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // here should lock the thread to write the file
            
            try
            {
                // define the taskFactory.
                //Control.CheckForIllegalCrossThreadCalls = false;
                SetDataTask();

                Stopwatch tic = new Stopwatch();
                
                OpenFileDialog invokeDialog = new OpenFileDialog();
                /*
                if (invokeDialog.ShowDialog() == DialogResult.OK)
                {
                    // open excel and read the excel
                    tic.Start();
                    string excelName = invokeDialog.FileName;
                    Excel.Application excel = new Excel.Application();
                    Excel.Workbook book = excel.Application.Workbooks.Add(excelName);
                    Excel.Worksheet sheet = book.ActiveSheet as Excel.Worksheet;

                    tic.Stop();
                    TimeSpan time = tic.Elapsed;
                    double milliseconds = time.TotalMilliseconds;
                    //assistantTBX.AppendText(milliseconds.ToString() + "\n");

                    sheet = (Excel.Worksheet)book.Worksheets.get_Item(1);   // get first sheet.
                    int row = sheet.UsedRange.Cells.Rows.Count;   // get Rows.
                    Excel.Range nodeNumber = sheet.Cells.get_Range("A2", "A" + row);
                    Excel.Range nodeXCoor = sheet.Cells.get_Range("C2", "C" + row);
                    Excel.Range nodeYCoor = sheet.Cells.get_Range("D2", "D" + row);
                    Excel.Range nodeKind = sheet.Cells.get_Range("E2", "E" + row);

                    // need to convert the excel to double.
                    object[,] arrayNumber = (object[,])nodeNumber.Value2;
                    object[,] arrayXCoor = (object[,])nodeXCoor.Value2;
                    object[,] arrayYCoor = (object[,])nodeYCoor.Value2;
                    object[,] arrayNodeKind = (object[,])nodeKind.Value2;

                    string[,] nodeKindString = new string[row - 1, 1];   // exclude the name row.
                    double[,] node = new double[row - 1, 3];
                    int flag1, flag2, flag3;   // flag1 to nodeNumber, flag2 to roadNumber, flag3 to outNumber.
                    flag1 = flag2 = flag3 = 0;


                    // finish reading data.
                    for (int i = 1; i < nodeKindString.GetLength(0); i++)
                    {
                        nodeKindString[i - 1, 0] = arrayNodeKind[i, 1].ToString();
                        node[i - 1, 0] = (double)arrayNumber[i, 1];
                        node[i - 1, 1] = (double)arrayXCoor[i, 1];
                        node[i - 1, 2] = (double)arrayYCoor[i, 1];
                    }
                    // init the number.
                    for (int i = 0; i < nodeKindString.GetLength(0); i++)
                    {
                        if (nodeKindString[i, 0] == "房屋结点")
                            flag1++;
                        else if (nodeKindString[i, 0] == "出口结点")
                            flag2++;
                        else if (nodeKindString[i, 0] == "一般结点")
                            flag3++;
                    }
                    map.nodeCoor = new float[flag1, 3];   // nodes' coor.
                    map.rodeCoor = new float[flag3, 3];   // rodes' coor.
                    map.outCoor = new float[flag2, 3];   // exit's coor.
                    flag1 = flag2 = flag3 = 0;

                    for (int i = 0; i < row - 1; i++)
                    {
                        if (nodeKindString[i, 0] == "房屋结点")
                        {
                            map.nodeCoor[flag1, 0] = (float)node[i, 0];
                            map.nodeCoor[flag1, 1] = (float)node[i, 1];
                            map.nodeCoor[flag1, 2] = (float)node[i, 2];
                            flag1++;
                        }
                        else if (nodeKindString[i, 0] == "出口结点")
                        {
                            map.outCoor[flag2, 0] = (float)node[i, 0];
                            map.outCoor[flag2, 1] = (float)node[i, 1];
                            map.outCoor[flag2, 2] = (float)node[i, 2];
                            flag2++;
                        }
                        else if (nodeKindString[i, 0] == "一般结点")
                        {
                            map.rodeCoor[flag3, 0] = (float)node[i, 0];
                            map.rodeCoor[flag3, 1] = (float)node[i, 1];
                            map.rodeCoor[flag3, 2] = (float)node[i, 2];
                            flag3++;
                        }
                    }

                    // normalized the coor of the nodes.
                    for (int i = 0; i < map.nodeCoor.GetLength(0); i++)
                    {
                        map.nodeCoor[i, 1] = (map.nodeCoor[i, 1] - map.xmin) * (map.xlen - 40) * 0.8f / (map.xmax - map.xmin) + map.xfmin + 30;   // x
                        map.nodeCoor[i, 2] = (-map.nodeCoor[i, 2] - map.ymin) * (map.ylen - 40) * 0.8f / (map.ymax - map.ymin) + map.yfmin + 40;   // y
                    }

                    for (int i = 0; i < map.outCoor.GetLength(0); i++)
                    {
                        map.outCoor[i, 1] = (map.outCoor[i, 1] - map.xmin) * (map.xlen - 40) * 0.8f / (map.xmax - map.xmin) + map.xfmin + 30;   // x
                        map.outCoor[i, 2] = (-map.outCoor[i, 2] - map.ymin) * (map.ylen - 40) * 0.8f / (map.ymax - map.ymin) + map.yfmin + 40;   // y
                    }
                    for (int i = 0; i < map.rodeCoor.GetLength(0); i++)
                    {
                        map.rodeCoor[i, 1] = (map.rodeCoor[i, 1] - map.xmin) * (map.xlen - 40) * 0.8f / (map.xmax - map.xmin) + map.xfmin + 30;   // x
                        map.rodeCoor[i, 2] = (-map.rodeCoor[i, 2] - map.ymin) * (map.ylen - 40) * 0.8f / (map.ymax - map.ymin) + map.yfmin + 40;   //y
                    }

                    excel.Visible = false;   // excel donot show up.
                    excel.Application.Quit();   // excel program ended.
                    KillExcel.KillExcelApp(excel);
                    // assistant codes area.
                    tic.Stop();
                    time = tic.Elapsed;
                    milliseconds = time.TotalMilliseconds;
                }
                */
            }
            catch (Exception error)
            {
                MessageBox.Show("error!");
                throw error;
            }
        }

        // interface quiting
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // ----------------------------- Algorithm Module ------------------------------------------------------
        // calculate the routes and write the evacuation plan file.
        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskFactory TFAll = new TaskFactory();
            // here is the algorithm part.
            // the AHP thread.
            // the Entropy thread.
            // the check thread.
            Task TAHP = TFAll.StartNew(CalculateSequenceTask_AHP);
            Task TEnt = TFAll.StartNew(CalculateSequenceTask_Entropy);

            // the AHP and Entropy thread begin at the same time.
            Task TCheck = TFAll.StartNew(CheckAHPandEntropyState);   // like the background thread to check whether it is over or not.

            // 
            Task TCalulateOrder = TCheck.ContinueWith(CalculateFinalSequence);   // with continueous function.
        }

        // check the data.
        private void routePlanningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display the result.
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                for (int j = 0; j < sequence.dicisionNumber; j++)
                {
                    assistant.AppendText((j + 1).ToString() + " : " + sequence.R[i, j].ToString("0.00") + "\n");
                }
                assistant.AppendText("\n");
            }

            /*
            assistant.AppendText("this is AHP criteria weight: \n");
            for(int i=0;i<sequence.criteriaNumber;i++)
            {
                assistant.AppendText(sequence.d_cri_eigVector[i, 0].ToString("0.000") + "\n");
            }
            assistant.AppendText("\n");

            */
            /*
            // display the weight.
            assistant.AppendText("this is AHP criteria weight: \n");
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                assistant.AppendText(sequence.d_cri_eigVector[i,0].ToString("0.000") + "\n");
            }
            assistant.AppendText("\n");

            assistant.AppendText("this is entrophy criteria weight: \n");
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                assistant.AppendText(sequence.entrophyCriteriaWeight[i].ToString("0.000") +"\n");
            }
            assistant.AppendText("\n");
            */

            assistant.AppendText("This is combination weight of the criteria. \n");
            for(int i=0;i<sequence.criteriaNumber;i++)
            {
                assistant.AppendText(sequence.eventualCriteriaWeight[i].ToString("0.000")+"\n");
            }

            // display the order.
            for (int i = 0; i < sequence.dicisionNumber; i++)
            {
                assistant.AppendText((i + 1).ToString() + " : " + sequence.eventualOrder[i].ToString() + "\n");
            }

        }



        // --- threads and tasks area ---
        // Here need to create a new class.
        // -- tasks --
        // 1. read the data tasks
        // this tasks should the most significant one.
        public void SetDataTask()
        {
            //map.xlen = tabPage1.Size.Width - 50;
            //map.ylen = tabPage1.Size.Height - 70;

            // Firstly, read the data form the data txt.
            StreamReader sr1 = new StreamReader(@"E:\Wuhan_University\Research\Sentific_Research\Evacuation_System\Junior_2nd\Program\Evacuation_Routine_Planning_System\data_raw.txt", true);
            string nexline = sr1.ReadToEnd();
            sr1.Close();

            map.xfmin = 10f;
            map.yfmin = 20f;
            map.xmin = 50f;
            map.ymin = 324.911f;
            map.xmax = 959.6790f;
            map.ymax = 880f;
            map.frameLength = 20f;
            sequence.dicisionNumber = 10;   // 10 Note
            sequence.criteriaNumber = 3;
            sequence.entrophyCriteriaWeight = new double[3];   // !!!!!!!!!!!!!!!!here is the define of the eventual data.
            sequence.ri = new double[2] { 0.52, 1.49 };   // 3 and 10

            // Criteria Matrix.
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!! the criteria weights needs to be adjust.
            // d_cri is the criteria 
            double[,] d_cri = new double[3, 3] { { 1, 2, 5 }, { 1.0/2.0, 1, 5 }, { 1.0 / 5.0, 1.0 / 5.0, 1 } };

            sequence.d_cri = new Matrix(d_cri);
            sequence.R = Matrix.Zeros(sequence.criteriaNumber, sequence.dicisionNumber);

            // Random initial number and go to the matrix.
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iRoot = BitConverter.ToInt16(buffer, 0);
            sequence.rdmNum = new Random(iRoot);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!here to initial the ramdom numbers.
            // here is the initial data, and read the data.
            // and here is a new thread, when using this thread to read the data, write file thread should be suspended. And lock the file.
            // when get it finished, the write thread can get the authority back.
            /*using (FileStream portDataReader = new FileStream(@"E:\Wuhan_University\Research\Sentific_Research\大学生科研项目\Junior_2nd\Program\Evacuation_Routine_Planning_System\Evacuation_Routine_Planning_System\Data.txt",FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
            {
                int dataLength = (int)portDataReader.Length;
                byte[] PortDatabyte = new byte[dataLength];
                int r = portDataReader.Read(PortDatabyte, 0, PortDatabyte.Length);
                string myStr = Encoding.UTF8.GetString(PortDatabyte);
                assistantTBX.Text = myStr;
            }

             */

            // random number
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                for (int j = 0; j < sequence.dicisionNumber; j++)
                {
                    sequence.R[i, j] = sequence.rdmNum.Next() / 100000000.0;
                }
            }

            // !!!!!!!!!!!!!!!!!!! set extreme number !!!!!!!!!!!!!!!!!!!!!!!!
            // set extreme data. Node 3;
            sequence.R[0, 0] = 16;
            sequence.R[0, 1] = 3;
            sequence.R[0, 2] = 50;
            sequence.R[0, 3] = 5;
            sequence.R[0, 4] = 11;
            sequence.R[0, 5] = 20;
            sequence.R[0, 6] = 50;
            sequence.R[0, 7] = 10;
            sequence.R[0, 8] = 9;
            sequence.R[0, 9] = 6;

            sequence.R[1, 0] = 10.87;
            sequence.R[1, 1] = 3.73;
            sequence.R[1, 2] = 12.3;
            sequence.R[1, 3] = 7.19;
            sequence.R[1, 4] = 2.89;
            sequence.R[1, 5] = 1.21;
            sequence.R[1, 6] = 11.56;
            sequence.R[1, 7] = 5.25;
            sequence.R[1, 8] = 3.05;
            sequence.R[1, 9] = 0.6;

            sequence.R[2, 0] = 12.12;
            sequence.R[2, 1] = 12.94;
            sequence.R[2, 2] = 3.91;
            sequence.R[2, 3] = 3.4;
            sequence.R[2, 4] = 19.41;
            sequence.R[2, 5] = 12.23;
            sequence.R[2, 6] = 16.47;
            sequence.R[2, 7] = 16.21;
            sequence.R[2, 8] = 9.72;
            sequence.R[2, 9] = 20.7;
            // --------------- entrophy global data --------------------
            sequence.Monitor = new bool[] { false, false, false, false, false, false };   // initially set 4 bools.
            sequence.infinateValue = new double[2];
            sequence.k = Math.Log(sequence.dicisionNumber);
            sequence.entropy = new double[sequence.criteriaNumber];
            sequence.entrophyCriteriaWeight = new double[sequence.criteriaNumber];
        }

        // 2. use the task to calculate AHP weight.
        public void CalculateSequenceTask_AHP()
        {
            // invoke the matrix.
            sequence.d = new Matrix[sequence.criteriaNumber];   // D with 3 number 
            sequence.d_eigVector = new Matrix[sequence.criteriaNumber];
            for (int i = 0; i < sequence.d.GetLength(0); i++)
            {
                sequence.d[i] = Matrix.Zeros(sequence.dicisionNumber, sequence.dicisionNumber);
                sequence.d_eigVector[i] = Matrix.Zeros(sequence.dicisionNumber, 1);
            }
            sequence.d_cri_eigValue = new MatrixEigenValue();
            sequence.d_cri_eigVector = Matrix.Zeros(sequence.criteriaNumber, 1);
            sequence.d_eigValue = new MatrixEigenValue[sequence.criteriaNumber];
            sequence.d_whole_weight = Matrix.Zeros(sequence.dicisionNumber, 1);

            // Positive Reciprocal Matrix.
            // create positive reciprocal matrix: dicision matrix.
            for (int i = 0; i < sequence.criteriaNumber; i++)
                for (int j = 0; j < sequence.dicisionNumber; j++)
                    for (int k = 0; k < sequence.dicisionNumber; k++)
                        sequence.d[i][j, k] = sequence.R[i,j] / sequence.R[i,k];

            // the eigvalue can be complex.
            sequence.d_eigValue[0] = sequence.d[0].Eigen();
            sequence.d_eigValue[1] = sequence.d[1].Eigen();
            sequence.d_eigValue[2] = sequence.d[2].Eigen();
            sequence.d_cri_eigValue = sequence.d_cri.Eigen();

            // new thread to transfer the eigvalue.
            ThreadPool.QueueUserWorkItem(EigValueOperationThread_1);   // start 1
            //ThreadPool.QueueUserWorkItem(EigValueOperationThread_2);   // start 2
            monitorAHPThread.WaitOne();
            //monitorAHPThread.WaitOne();
            // here we need to set the weight of the criteria, which is the basic wight.
            /*
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                sequence.d_whole_weight += sequence.d_cri_eigVector[i, 0] * sequence.d_eigVector[i];
            }
            */

            sequence.Monitor[4] = true;
        }
        
        // 3. use the task to calculate Entropy weight.
        public void CalculateSequenceTask_Entropy()
        {
            // non-dimensinalize
            R = sequence.R.Transpose();   // R :34*3 matrix;

            // non-dimensionalize operation.
            for(int i=0;i<R.ColumnCount;i++)
            {
                sequence.infinateValue[0] = GetMaxValue(i,R, "max");
                sequence.infinateValue[1] = GetMaxValue(i,R, "min");

                // Error 1: normalization with wrong.
                for(int j=0;j<R.RowCount;j++)
                {
                    R[j, i] = (R[j, i] - sequence.infinateValue[1]) / (sequence.infinateValue[0] - sequence.infinateValue[1]);
                }
            }

            // calculate the criteria entrophy
            for(int i=0;i<sequence.criteriaNumber;i++)
            {
                sequence.entropy[i]=0;
                for(int j=0;j<sequence.dicisionNumber;j++)
                {
                    sequence.entropy[i] += -sequence.k * CalculateEntrophy(R[j, i]);
                }
                sequence.entropy[i] = 1 - sequence.entropy[i];
            }

            // calculate the criteria weight.
            double templess;
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                templess = Sum(sequence.entropy);
                sequence.entrophyCriteriaWeight[i] = sequence.entropy[i] / templess;
            }

            sequence.Monitor[5] = true;
        }

        // 4. use the task to check whether the AHP and Entropy ended.
        public void CheckAHPandEntropyState()
        {
            bool tempFlag=true;
            while(tempFlag)
            {
                if (sequence.Monitor[4] & sequence.Monitor[5])   // check whether AHP and entropy are over.
                    tempFlag = false;
            }
        }

        // 5. use the task to calculate the final order of every note.
        public void CalculateFinalSequence(Task t)
        {
            sequence.eventualOrder = new int[sequence.dicisionNumber];
            sequence.eventualWeight = new double[sequence.dicisionNumber];
            sequence.eventualCriteriaWeight = new double[sequence.criteriaNumber];

            // calculate the final criteria weight.
            for (int i = 0; i < sequence.criteriaNumber; i++)
                sequence.eventualCriteriaWeight[i] = sequence.entrophyCriteriaWeight[i] * 0.5 + sequence.d_cri_eigVector[i, 0] * 0.5;

            // calculate the final dicision weight. Using the normalized Data R.
            for (int i = 0; i < sequence.dicisionNumber; i++)
            {
                for (int j = 0; j < sequence.criteriaNumber; j++)
                {
                    sequence.eventualWeight[i] += R[i, j] * sequence.eventualCriteriaWeight[j];
                }
            }


            // when the sequence will not change.
            List<double> eventualWeightList = new List<double>(sequence.dicisionNumber);   // set eventual weight list.

            for (int i = 0; i < eventualWeightList.Capacity; i++)
            {
                eventualWeightList.Add(sequence.eventualWeight[i]);
            }

            eventualWeightList.Sort();   // sort
            eventualWeightList.Reverse();   // reverse.
            
            // get the final order.
            for (int i = 0; i < sequence.dicisionNumber; i++)
            {
                sequence.eventualOrder[i] = eventualWeightList.IndexOf(sequence.eventualWeight[i]);
            }
            
            MessageBox.Show("Sequence caluculated.");
        }

        // -- threads --
        // aim: to find max eigValue.
        public void EigValueOperationThread_1(object state)
        {
            double maxEigValue;
            maxEigValue = GetMaxEigValue(sequence.d_cri_eigValue);

            // calculate the eigVector
            sequence.d_cri_eigVector = GetEigVector(sequence.d_cri, "和法");
            monitorAHPThread.Set();
        }

        // aim: to find max eigValue and get out of the eigvector.
        public void EigValueOperationThread_2(object state)
        {
            double[] maxEigValue = new double[sequence.criteriaNumber];

            // find the maximum eigValue.
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                maxEigValue[i] = GetMaxEigValue(sequence.d_eigValue[i]);
            }

            // Calculate the eigVector.
            for (int i = 0; i < sequence.criteriaNumber; i++)
            {
                sequence.d_eigVector[i] = GetEigVector(sequence.d[i], "和法");
            }

            monitorAHPThread.Set();
        }
   
        // -- assistant function area --
        // normalized the node. !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        private static float[,] NormalizedMapNode(float[,] input)
        {
            float[,] a=new float[input.GetLength(0),input.GetLength(1)];
            return a;
        }

        // calculate the eigVector.
        private static Matrix GetEigVector(Matrix matrix,string type)
        {
            int row = matrix.RowCount;
            int column = matrix.ColumnCount;
            double[] Sum_column = new double[column];
            Matrix w = Matrix.Zeros(matrix.RowCount, 1);

            if (type == "和法")
            {
                for (int i = 0; i < column; i++)
                {
                    Sum_column[i] = 0;
                    for (int j = 0; j < row; j++)
                    {
                        Sum_column[i] += matrix[j,i];
                    }
                }

                //进行归一化,计算特征向量W

                for (int i = 0; i < row; i++)
                {
                    w[i,0] = 0;
                    for (int j = 0; j < column; j++)
                    {
                        w[i,0] += matrix[i,j] / Sum_column[j];
                    }
                    w[i,0] /= row;
                }
            }

            if (type == "根法")
            {
                for (int i = 0; i < column; i++)
                {
                    Sum_column[i] = 0;
                    for (int j = 0; j < row; j++)
                    {
                        Sum_column[i] += matrix[j,i];
                    }
                }

                //进行归一化,计算特征向量W
                double sum = 0;
                for (int i = 0; i < row; i++)
                {
                    w[i,0] = 1;
                    for (int j = 0; j < column; j++)
                    {
                        w[i,0] *= matrix[i,j] / Sum_column[j];
                    }

                    w[i,0] = Math.Pow(w[i,0], 1.0 / row);
                    sum += w[i,0];
                }

                for (int i = 0; i < row; i++)
                {
                    w[i,0] /= sum;
                }
            }

            if (type == "幂法")
            {
                double[] w0 = new double[row];
                for (int i = 0; i < row; i++)
                {
                    w0[i] = 1.0 / row;
                }

                //一般向量W（k+1）
                double[] w1 = new double[row];
                //W（k+1）的归一化向量                
                double sum = 1.0;
                double d = 1.0;
                double delt = 0.00001;
                while (d > delt)
                {
                    d = 0.0;
                    sum = 0;

                    //获取向量
                    for (int j = 0; j < row; j++)
                    {
                        w1[j] = 0;
                        for (int k = 0; k < row; k++)
                        {
                            w1[j] += matrix[j,k] * w0[k];
                        }
                        sum += w1[j];
                    }

                    //向量归一化 
                    for (int k = 0; k < row; k++)
                    {
                        w[k,0] = w1[k] / sum;
                        d = Math.Max(Math.Abs(w[k,0] - w0[k]), d);//最大差值
                        w0[k] = w[k,0];//用于下次迭代使用 
                    }
                }
            }
            return w;
        }

        // select the max eigValue.
        private static double GetMaxEigValue(MatrixEigenValue inputMatrix)
        {
            double maxEigValue;
            List<double> eigList = new List<double>(inputMatrix.Real.Length);
            eigList.Clear();
            for (int i = 0; i < eigList.Capacity; i++)
            {
                eigList.Add(inputMatrix.Real[i]);
            }
            eigList.Sort();
            maxEigValue = eigList[inputMatrix.Real.Length-1];
            return maxEigValue;
        }

        // select the max or min value in a matrix
        private static double GetMaxValue(int flag,Matrix inputMatrix, string temp)
        {
            List<double> eigList = new List<double>(inputMatrix.RowCount);
            eigList.Clear();
            for (int i = 0; i < eigList.Capacity; i++)
            {
                eigList.Add(inputMatrix[i,flag]);
            }
            eigList.Sort();
            if (temp == "max")
            {
                return eigList[inputMatrix.RowCount - 1];
            }
            else if(temp == "min")
            {
                return eigList[0];
            }
            else
            {
                MessageBox.Show("Error input in get max value, the result is invalid.");
                return 0.0;
            }
        }

        // normalized operations
        private static Matrix NormalizedOperation(Matrix inputMatrix,int index)
        {       
            Matrix resultMatrix = Matrix.Zeros(inputMatrix.RowCount,inputMatrix.ColumnCount);
            Matrix tempMatrix;
            if(index==0)   // row superior and normalized.
            {
                tempMatrix = Matrix.Zeros(1, inputMatrix.ColumnCount);
                for (int i = 0; i < inputMatrix.RowCount; i++)
                {
                    for (int j = 0; j < inputMatrix.ColumnCount; j++)
                        tempMatrix[0, j] = inputMatrix[i, j];
                    for (int j = 0; j < inputMatrix.ColumnCount; j++)
                        resultMatrix[i, j] = inputMatrix[i, j] / tempMatrix.Sum();
                }
            }
            else if(index==1) //column superior and normalized.
            {
                tempMatrix = Matrix.Zeros(inputMatrix.RowCount,1);
                for (int i = 0; i < inputMatrix.ColumnCount; i++)
                {
                    for (int j = 0; j < inputMatrix.RowCount; j++)
                        tempMatrix[j,0] = inputMatrix[j,i];
                    for (int j = 0; j < inputMatrix.RowCount; j++)
                        resultMatrix[j,i] = inputMatrix[j,i] / tempMatrix.Sum();
                }
            }
            else
            {
                MessageBox.Show("Error input to normalize the matrix.");
            }

            return resultMatrix;
        }

        // calculate entrophy weight
        private static double CalculateEntrophy(double input)
        {
            if (input == 0.0)
                input += 0.000000001;
            if (input < 0)
                MessageBox.Show("Error computation number");
            return input * Math.Log(input);
        }

        // sum up
        private static double Sum(double [] input)
        {
            double res = 0 ;
            for(int i=0;i<input.GetLength(0);i++)
            {
                res += input[i];
            }
            return res;
        }


        // ------------------------------- Monitor Module ----------------------------------
        // Spontaneously mapping and refreshing.
        private void mappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Glut.glutInit();        // Initialize glut this is the main cord.

            // Setup display mode to double buffer and RGB color
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_RGB);
            // Set the screen size
            Glut.glutInitWindowSize(600, 600);
            Glut.glutCreateWindow("OpenGL 3D Navigation Program With Tao");
            init();
            Glut.glutReshapeFunc(reshape);
            Glut.glutDisplayFunc(drawings);
            // Set window's key callback
            Glut.glutKeyboardFunc(new Glut.KeyboardCallback(keyboard));
            // Set window's to specialKey callback   
            Glut.glutSpecialFunc(new Glut.SpecialCallback(specialKey));
            // Set window's to Mouse callback
            Glut.glutMouseFunc(new Glut.MouseCallback(processMouseActiveMotion));
            // Set window's to motion callback
            Glut.glutMotionFunc(new Glut.MotionCallback(processMouse));
            // Set window's to mouse motion callback
            Glut.glutMouseWheelFunc(new Glut.MouseWheelCallback(processMouseWheel));
            Glut.glutMainLoop();

        }

        // Draw the lines (x,y,z)
        static void drawings()
        {
            // Clear the Color Buffer and Depth Buffer
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glPushMatrix();   // It is important to push the Matrix before 
                                 // calling glRotatef and glTranslatef
            Gl.glRotatef(rotX, 1.0f, 0.0f, 0.0f);            // Rotate on x
            Gl.glRotatef(rotY, 0.0f, 1.0f, 0.0f);            // Rotate on y
            Gl.glRotatef(rotZ, 0.0f, 0.0f, 1.0f);            // Rotate on z

            if (rotation) // If F2 is pressed update x,y,z for rotation of the cube
            {
                rotX += 0.2f;
                rotY += 0.2f;
                rotZ += 0.2f;
            }

            Gl.glTranslatef(X, Y, Z);        // Translates the screen left or right, 
                                             // up or down or zoom in zoom out

            if (lines)  // If F1 is pressed don't draw the lines
            {
                // Draw the positive side of the lines x,y,z
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3f(0.0f, 1.0f, 0.0f);                // Green for x axis
                Gl.glVertex3f(0f, 0f, 0f);
                Gl.glVertex3f(10f, 0f, 0f);
                Gl.glColor3f(1.0f, 0.0f, 0.0f);                // Red for y axis
                Gl.glVertex3f(0f, 0f, 0f);
                Gl.glVertex3f(0f, 10f, 0f);
                Gl.glColor3f(0.0f, 0.0f, 1.0f);                // Blue for z axis
                Gl.glVertex3f(0f, 0f, 0f);
                Gl.glVertex3f(0f, 0f, 10f);
                Gl.glEnd();

                // Dotted lines for the negative sides of x,y,z coordinates
                Gl.glEnable(Gl.GL_LINE_STIPPLE); // Enable line stipple to use a 
                                                 // dotted pattern for the lines
                Gl.glLineStipple(1, 0x0101);     // Dotted stipple pattern for the lines
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3f(0.0f, 1.0f, 0.0f);                    // Green for x axis
                Gl.glVertex3f(-10f, 0f, 0f);
                Gl.glVertex3f(0f, 0f, 0f);
                Gl.glColor3f(1.0f, 0.0f, 0.0f);                    // Red for y axis
                Gl.glVertex3f(0f, 0f, 0f);
                Gl.glVertex3f(0f, -10f, 0f);
                Gl.glColor3f(0.0f, 0.0f, 1.0f);                    // Blue for z axis
                Gl.glVertex3f(0f, 0f, 0f);
                Gl.glVertex3f(0f, 0f, -10f);
                Gl.glEnd();
            }

            // I start to draw my 3D cube
            Gl.glBegin(Gl.GL_POLYGON);
            // I'm setting a new color for each corner, this creates a rainbow effect
            Gl.glColor3f(0.0f, 0.0f, 1.0f);             // Set color to blue
            Gl.glVertex3f(5.0f, 3.0f, 0.0f);
            Gl.glColor3f(1.0f, 0.0f, 0.0f);             // Set color to red
            Gl.glVertex3f(-2.5f, 3.0f, 0.0f);
            Gl.glColor3f(0.0f, 1.0f, 0.0f);             // Set color to green
            Gl.glVertex3f(-10.0f, -5.0f, 5.0f);
            Gl.glColor3f(1.0f, 0.0f, 1.0f);     // Set color to som                 
            Gl.glVertex3f(-2.5f, -5.0f, 5.0f);
            Gl.glEnd();


            Gl.glDisable(Gl.GL_LINE_STIPPLE);   // Disable the line stipple
            Glut.glutPostRedisplay();           // Redraw the scene
            Gl.glPopMatrix();                   // Don't forget to pop the Matrix
            Glut.glutSwapBuffers();
        }

        // Initialize the OpenGL window
        static void init()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH);     // Set the shading model to smooth 
            Gl.glClearColor(0, 0, 0, 0.0f);    // Clear the Color
            // Clear the Color and Depth Buffer
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearDepth(1.0f);          // Set the Depth buffer value (ranges[0,1])
            Gl.glEnable(Gl.GL_DEPTH_TEST);  // Enable Depth test
            Gl.glDepthFunc(Gl.GL_LEQUAL);   // If two objects on the same coordinate 
                                            // show the first drawn
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
        }

        // This function is called whenever the window size is changed
        static void reshape(int w, int h)
        {
            Gl.glViewport(0, 0, w, h);                // Set the viewport
            Gl.glMatrixMode(Gl.GL_PROJECTION);        // Set the Matrix mode
            Gl.glLoadIdentity();
            Glu.gluPerspective(75f, (float)w / (float)h, 0.10f, 500.0f);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(rotLx, rotLy, 15.0f +
                     rotLz, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
        }

        // This function is used for the navigation keys
        public static void keyboard(byte key, int x, int y)
        {
            switch (key)
            {
                // x,X,y,Y,z,Z uses the glRotatef() function
                case 120:    // x             // Rotates screen on x axis 
                    rotX -= 2.0f;
                    break;
                case 88:    // X            // Opposite way 
                    rotX += 2.0f;
                    break;
                case 121:    // y            // Rotates screen on y axis
                    rotY -= 2.0f;
                    break;
                case 89:    // Y            // Opposite way
                    rotY += 2.0f;
                    break;
                case 122:    // z            // Rotates screen on z axis
                    rotZ -= 2.0f;
                    break;
                case 90:    // Z            // Opposite way
                    rotZ += 2.0f;
                    break;

                // j,J,k,K,l,L uses the gluLookAt function for navigation
                case 106:   // j
                    rotLx -= 2.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0 + rotLz,
                        0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                    break;
                case 74:    // J
                    rotLx += 2.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0 + rotLz,
                        0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                    break;
                case 107:   // k
                    rotLy -= 2.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0 + rotLz,
                        0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                    break;
                case 75:    // K
                    rotLy += 2.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0 + rotLz,
                        0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                    break;
                case 108: // (l) It has a special case when the rotLZ becomes 
                          // less than -15 the screen is viewed from the opposite side
                          // therefore this if statement below does not allow 
                          // rotLz be less than -15
                    if (rotLz + 14 >= 0)
                        rotLz -= 2.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0 + rotLz,
                        0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                    break;
                case 76:    // L
                    rotLz += 2.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0 + rotLz,
                        0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                    break;
                case 98:    // b        // Rotates on x axis by -90 degree
                    rotX -= 90.0f;
                    break;
                case 66:    // B        // Rotates on y axis by 90 degree
                    rotX += 90.0f;
                    break;
                case 110:    // n        // Rotates on y axis by -90 degree
                    rotY -= 90.0f;
                    break;
                case 78:    // N        // Rotates on y axis by 90 degree
                    rotY += 90.0f;
                    break;
                case 109:    // m        // Rotates on z axis by -90 degree
                    rotZ -= 90.0f;
                    break;
                case 77:    // M        // Rotates on z axis by 90 degree
                    rotZ += 90.0f;
                    break;
                case 111:    // o        // Resets all parameters
                case 80:    // O        // Displays the cube in the starting position
                    rotation = false;
                    X = Y = 0.0f;
                    Z = 0.0f;
                    rotX = 0.0f;
                    rotY = 0.0f;
                    rotZ = 0.0f;
                    rotLx = 0.0f;
                    rotLy = 0.0f;
                    rotLz = 0.0f;
                    Gl.glMatrixMode(Gl.GL_MODELVIEW);
                    Gl.glLoadIdentity();
                    Glu.gluLookAt(rotLx, rotLy, 15.0f + rotLz,
                        0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
                    break;
            }
            Glut.glutPostRedisplay();    // Redraw the scene
        }

        // Called on special key pressed
        private static void specialKey(int key, int x, int y)
        {
            // Check which key is pressed
            switch (key)
            {
                case Glut.GLUT_KEY_LEFT:    // Rotate on x axis
                    X -= 2.0f;
                    break;
                case Glut.GLUT_KEY_RIGHT:    // Rotate on x axis (opposite)
                    X += 2.0f;
                    break;
                case Glut.GLUT_KEY_UP:        // Rotate on y axis 
                    Y += 2.0f;
                    break;
                case Glut.GLUT_KEY_DOWN:    // Rotate on y axis (opposite)
                    Y -= 2.0f;
                    break;
                case Glut.GLUT_KEY_PAGE_UP:  // Rotate on z axis
                    Z -= 2.0f;
                    break;
                case Glut.GLUT_KEY_PAGE_DOWN:// Rotate on z axis (opposite)
                    Z += 2.0f;
                    break;
                case Glut.GLUT_KEY_F1:      // Enable/Disable coordinate lines
                    lines = !lines;
                    break;
                case Glut.GLUT_KEY_F2:      // Enable/Disable automatic rotation
                    rotation = !rotation;
                    break;
                default:
                    break;
            }
            Glut.glutPostRedisplay();        // Redraw the scene
        }

        // Capture the mouse click event 
        static void processMouseActiveMotion(int button, int state, int x, int y)
        {
            mousePressed = button;          // Capture which mouse button is down
            old_x = x;                      // Capture the x value
            old_y = y;                      // Capture the y value
        }

        // Translate the x,y windows coordinates to OpenGL coordinates
        static void processMouse(int x, int y)
        {
            if ((mousePressed == 0))    // If left mouse button is pressed
            {
                X = (x - old_x) / 15;       // I did divide by 15 to adjust 
                                            // for a nice translation 
                Y = -(y - old_y) / 15;
            }

            Glut.glutPostRedisplay();
        }

        // Get the mouse wheel direction
        static void processMouseWheel(int wheel, int direction, int x, int y)
        {

            Z += direction;  // Adjust the Z value 
            Glut.glutPostRedisplay();
        }


    }

    // -------------------------- Program Optimized Function --------------------------------

    // 1. kill excel program.
    public static class KillExcel
    {
        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        public static void KillExcelApp(this Excel.Application app)
        {
            app.Quit();
            IntPtr intprt = new IntPtr(app.Hwnd);
            int id;
            GetWindowThreadProcessId(intprt, out id);
            var p = Process.GetProcessById(id);
            p.Kill();
        }
    }

}