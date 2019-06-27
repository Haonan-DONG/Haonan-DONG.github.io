using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinearAlgebra;

namespace Evacuation_Routine_Planning_System
{
    class Global
    {
        public static bool Login;

        public struct Sequence
        {
            // ----------------------- AHP parameters ----------------------
            public Matrix[] d;   // reciprocal matrix
            public MatrixEigenValue[] d_eigValue;
            public Matrix[] d_eigVector;
            public Matrix d_cri, d_cri_eigVector;   // here is the criteria to the aim matrix and the weight vector.
            public MatrixEigenValue d_cri_eigValue;   
            public Matrix d_whole_weight;
            public int dicisionNumber;
            public int criteriaNumber;
            public double[] ri;

            // ----------------------- Entropy parameters ---------------------
            public double k;
            public double[] entropy;
            public Matrix R;   // the whole criteria data clauster. criteria * discision
            // Remember! R is the data matrix.
            public double[] infinateValue;
            public double[] entrophyCriteriaWeight;

            // ----------------------- Other parameters -----------------------
            public Random rdmNum;   // now we use random data to calculate the sequence.
            public bool[] Monitor;
            public double[] eventualCriteriaWeight;
            public double[] eventualWeight;
            public int[] eventualOrder;
            
        }

        public struct Mapping
        {
            public  float[,] nodeCoor;   // nodes' coor.
            public  float[,] rodeCoor;   // rodes' coor.
            public  float[,] outCoor;   // exit's coor.
            public  int[,] routinePlan;   // this needs a list
            public  float xlen, ylen, xmin, ymin, xmax, ymax, xfmin, yfmin;
            public float frameLength;
        }
    }
}
