﻿namespace Undersoft.SDK.Estimating
{
    public class BackPropagationNeuralNetworkEstimator
    {
        private int numInput;
        private int numHidden;
        private int numOutput;

        private double[] inputs;
        private double[][] ihWeights; // input-to-hidden
        private double[] hBiases;
        private double[] hSums;
        private double[] hOutputs;

        private double[][] hoWeights;  // hidden-to-output
        private double[] oBiases;
        private double[] oSums;
        private double[] outputs;

        private double[] oGrads; // output gradients for back-propagation
        private double[] hGrads; // hidden gradients for back-propagation

        private double[][] ihPrevWeightsDelta;  // for momentum with back-propagation
        private double[] hPrevBiasesDelta;
        private double[][] hoPrevWeightsDelta;
        private double[] oPrevBiasesDelta;

        public BackPropagationNeuralNetworkEstimator(int numInput, int numHidden, int numOutput)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;

            inputs = new double[numInput];
            ihWeights = CreateMatrix(numInput, numHidden);
            hBiases = new double[numHidden];
            hSums = new double[numHidden];

            hOutputs = new double[numHidden];
            hoWeights = CreateMatrix(numHidden, numOutput);
            oBiases = new double[numOutput];
            oSums = new double[numOutput];
            outputs = new double[numOutput];

            oGrads = new double[numOutput];
            hGrads = new double[numHidden];

            ihPrevWeightsDelta = CreateMatrix(numInput, numHidden);
            hPrevBiasesDelta = new double[numHidden];
            hoPrevWeightsDelta = CreateMatrix(numHidden, numOutput);
            oPrevBiasesDelta = new double[numOutput];
        }

        public void SetWeights(double[] weights)
        {
            // assumes weights[] has order: input-to-hidden wts, hidden biases, hidden-to-output wts, output biases
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + numHidden + numOutput;
            if (weights.Length != numWeights)
                throw new Exception("The weights vector length: " + weights.Length +
                  " does not match the total number of weights and biases: " + numWeights);

            int k = 0; // points into weights param

            for (int i = 0; i < numInput; ++i)
                for (int j = 0; j < numHidden; ++j)
                    ihWeights[i][j] = weights[k++];

            for (int i = 0; i < numHidden; ++i)
                hBiases[i] = weights[k++];

            for (int i = 0; i < numHidden; ++i)
                for (int j = 0; j < numOutput; ++j)
                    hoWeights[i][j] = weights[k++];

            for (int i = 0; i < numOutput; ++i)
                oBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            int numWeights = (numInput * numHidden) + (numHidden * numOutput) + numHidden + numOutput;
            double[] result = new double[numWeights];
            int k = 0;
            for (int i = 0; i < ihWeights.Length; ++i)
                for (int j = 0; j < ihWeights[0].Length; ++j)
                    result[k++] = ihWeights[i][j];
            for (int i = 0; i < hBiases.Length; ++i)
                result[k++] = hBiases[i];
            for (int i = 0; i < hoWeights.Length; ++i)
                for (int j = 0; j < hoWeights[0].Length; ++j)
                    result[k++] = hoWeights[i][j];
            for (int i = 0; i < oBiases.Length; ++i)
                result[k++] = oBiases[i];
            return result;
        }

        public double[] GetOutputs()
        {
            double[] result = new double[numOutput];
            this.outputs.CopyTo(result, 0);
            return result;
        }

        public double[] ComputeOutputs(double[] xValues)
        {
            if (xValues.Length != numInput)
                throw new Exception("Inputs vector length " + inputs.Length + " does not match NN numInput value " + numInput);

            for (int i = 0; i < numHidden; ++i)
                hSums[i] = 0.0;
            for (int i = 0; i < numOutput; ++i)
                oSums[i] = 0.0;

            for (int i = 0; i < xValues.Length; ++i) // copy x-values to inputs
                this.inputs[i] = xValues[i];

            for (int j = 0; j < numHidden; ++j)  // compute hidden layer weighted sums
                for (int i = 0; i < numInput; ++i)
                    hSums[j] += this.inputs[i] * ihWeights[i][j];

            for (int i = 0; i < numHidden; ++i)  // add biases to hidden sums
                hSums[i] += hBiases[i];

            for (int i = 0; i < numHidden; ++i)   // apply tanh activation
                hOutputs[i] = HyperTanFunction(hSums[i]);

            for (int j = 0; j < numOutput; ++j)   // compute output layer weighted sums
                for (int i = 0; i < numHidden; ++i)
                    oSums[j] += hOutputs[i] * hoWeights[i][j];

            for (int i = 0; i < numOutput; ++i)  // add biases to output sums
                oSums[i] += oBiases[i];

            for (int i = 0; i < numOutput; ++i)   // apply log-sigmoid activation
                this.outputs[i] = SigmoidFunction(oSums[i]);

            //double[] result = new double[numOutput]; // for convenience when calling method
            //this.outputs.CopyTo(result, 0);
            return this.outputs;
        } // ComputeOutputs

        private static double SigmoidFunction(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        private static double HyperTanFunction(double x)
        {
            if (x < -45.0) return -1.0;
            else if (x > 45.0) return 1.0;
            else return Math.Tanh(x);
        }

        public void UpdateWeights(double[] tValues, double learn, double mom) // back-propagation
        {
            // assumes that SetWeights and ComputeOutputs have been called and so inputs and outputs have values
            if (tValues.Length != numOutput)
                throw new Exception("target values not same Length as output in UpdateWeights");

            // 1. compute output gradients. assumes log-sigmoid!
            for (int i = 0; i < oGrads.Length; ++i)
            {
                double derivative = (1 - outputs[i]) * outputs[i]; // derivative of log-sigmoid is y(1-y)
                oGrads[i] = derivative * (tValues[i] - outputs[i]); // oGrad = (1 - O)(O) * (T-O)
            }

            // 2. compute hidden gradients. assumes tanh!
            for (int i = 0; i < hGrads.Length; ++i)
            {
                double derivative = (1 - hOutputs[i]) * (1 + hOutputs[i]); // derivative of tanh is (1-y)(1+y)
                double sum = 0.0;
                for (int j = 0; j < numOutput; ++j) // each hidden delta is the sum of numOutput terms
                    sum += oGrads[j] * hoWeights[i][j]; // each downstream gradient * outgoing weight
                hGrads[i] = derivative * sum; // hGrad = (1-O)(1+O) * E(oGrads*oWts)
            }

            // 3. update input to hidden weights (gradients must be computed right-to-left but weights can be updated in any order)
            for (int i = 0; i < ihWeights.Length; ++i) // 0..2 (3)
            {
                for (int j = 0; j < ihWeights[0].Length; ++j) // 0..3 (4)
                {
                    double delta = learn * hGrads[j] * inputs[i]; // compute the new delta = "eta * hGrad * input"
                    ihWeights[i][j] += delta; // update
                    ihWeights[i][j] += mom * ihPrevWeightsDelta[i][j]; // add momentum using previous delta. on first pass old value will be 0.0 but that's OK.
                    ihPrevWeightsDelta[i][j] = delta; // save the delta for next time
                }
            }

            // 4. update hidden biases
            for (int i = 0; i < hBiases.Length; ++i)
            {
                double delta = learn * hGrads[i] * 1.0; // the 1.0 is the constant input for any bias; could leave out
                hBiases[i] += delta;
                hBiases[i] += mom * hPrevBiasesDelta[i];
                hPrevBiasesDelta[i] = delta; // save delta
            }

            // 5. update hidden to output weights
            for (int i = 0; i < hoWeights.Length; ++i)  // 0..3 (4)
            {
                for (int j = 0; j < hoWeights[0].Length; ++j) // 0..1 (2)
                {
                    double delta = learn * oGrads[j] * hOutputs[i];  // hOutputs are inputs to next layer
                    hoWeights[i][j] += delta;
                    hoWeights[i][j] += mom * hoPrevWeightsDelta[i][j];
                    hoPrevWeightsDelta[i][j] = delta;
                }
            }

            // 6. update hidden to output biases
            for (int i = 0; i < oBiases.Length; ++i)
            {
                double delta = learn * oGrads[i] * 1.0;
                oBiases[i] += delta;
                oBiases[i] += mom * oPrevBiasesDelta[i];
                oPrevBiasesDelta[i] = delta;
            }
        } // UpdateWeights

        public static double[][] CreateMatrix(int rows, int cols)
        {
            return (double[][])Array.CreateInstance(typeof(double), rows, cols);
        }
    }
}
