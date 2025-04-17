using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ConductivityCalculator : MonoBehaviour
{
    private int[,] connectionMatrix; // conmat1
    private int[,] voltageMatrix; // conmat2
    private int[,] laplacianMatrix; // conmat3
    private List<int> startNodes; // Nodes with S (1V)
    private List<int> finalNodes; // Nodes with F (0V)
    private List<int> otherNodes; // Nodes with no voltage boundary
    private float rcont = 200f; // Contact resistance

    public void CalculateConductivity(int[,] conmat1, List<int> startIndices, List<int> finalIndices)
    {
        // Initialize matrices
        int n = conmat1.GetLength(0);
        connectionMatrix = conmat1;
        voltageMatrix = new int[n + 2, n + 2]; // +2 for S and F nodes
        laplacianMatrix = new int[n + 2, n + 2];

        // Store node indices
        startNodes = startIndices;
        finalNodes = finalIndices;
        otherNodes = Enumerable.Range(0, n)
            .Where(i => !startIndices.Contains(i) && !finalIndices.Contains(i))
            .ToList();

        // Create voltage matrix (conmat2)
        // First row: S node connections
        voltageMatrix[0, 0] = 0;
        for (int i = 0; i < otherNodes.Count; i++)
        {
            int sum = 0;
            foreach (int startNode in startNodes)
            {
                sum += connectionMatrix[startNode, otherNodes[i]];
            }
            voltageMatrix[0, i + 1] = sum;
        }
        voltageMatrix[0, n + 1] = 0;

        // Middle rows: Other node connections
        for (int i = 0; i < otherNodes.Count; i++)
        {
            // First column: Connections to S nodes
            int sumS = 0;
            foreach (int startNode in startNodes)
            {
                sumS += connectionMatrix[otherNodes[i], startNode];
            }
            voltageMatrix[i + 1, 0] = sumS;

            // Middle columns: Connections between other nodes
            for (int j = 0; j < otherNodes.Count; j++)
            {
                voltageMatrix[i + 1, j + 1] = connectionMatrix[otherNodes[i], otherNodes[j]];
            }

            // Last column: Connections to F nodes
            int sumF = 0;
            foreach (int finalNode in finalNodes)
            {
                sumF += connectionMatrix[otherNodes[i], finalNode];
            }
            voltageMatrix[i + 1, n + 1] = sumF;
        }

        // Last row: F node connections
        voltageMatrix[n + 1, 0] = 0;
        for (int i = 0; i < otherNodes.Count; i++)
        {
            int sum = 0;
            foreach (int finalNode in finalNodes)
            {
                sum += connectionMatrix[finalNode, otherNodes[i]];
            }
            voltageMatrix[n + 1, i + 1] = sum;
        }
        voltageMatrix[n + 1, n + 1] = 0;

        // Create Laplacian matrix (conmat3)
        for (int i = 0; i < n + 2; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < n + 2; j++)
            {
                rowSum += voltageMatrix[i, j];
            }
            for (int j = 0; j < n + 2; j++)
            {
                laplacianMatrix[i, j] = -voltageMatrix[i, j];
                if (i == j)
                {
                    laplacianMatrix[i, j] += rowSum;
                }
            }
        }

        // Save matrices to CSV files
        SaveMatrixToCSV(connectionMatrix, "ConnectionMatrix.csv");
        SaveMatrixToCSV(voltageMatrix, "VoltageMatrix.csv");
        SaveMatrixToCSV(laplacianMatrix, "LaplacianMatrix.csv");

        // Calculate conductivity
        float conductivity = CalculateConductivityFromMatrices();
        Debug.Log($"Calculated Conductivity: {conductivity}");
    }

    private float CalculateConductivityFromMatrices()
    {
        int n = laplacianMatrix.GetLength(0);
        
        // Create matrix A and vector b for solving Ax = b
        float[,] A = new float[n - 2, n - 2];
        float[] b = new float[n - 2];

        // Fill matrix A (excluding first and last rows/columns)
        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < n - 1; j++)
            {
                A[i - 1, j - 1] = laplacianMatrix[i, j];
            }
        }

        // Fill vector b
        for (int i = 1; i < n - 1; i++)
        {
            b[i - 1] = -laplacianMatrix[i, 0]; // First column (S node)
        }

        // Add contact resistance term
        A[n - 3, n - 3] += rcont;

        // Solve the system (using a simple Gaussian elimination)
        float[] x = SolveLinearSystem(A, b);

        // Calculate total current (last element of x)
        float totalCurrent = x[n - 3];

        // Calculate conductivity (1/resistance)
        float conductivity = 1.0f / totalCurrent;

        return conductivity;
    }

    private float[] SolveLinearSystem(float[,] A, float[] b)
    {
        int n = b.Length;
        float[] x = new float[n];

        // Forward elimination
        for (int k = 0; k < n - 1; k++)
        {
            for (int i = k + 1; i < n; i++)
            {
                float factor = A[i, k] / A[k, k];
                for (int j = k; j < n; j++)
                {
                    A[i, j] -= factor * A[k, j];
                }
                b[i] -= factor * b[k];
            }
        }

        // Back substitution
        x[n - 1] = b[n - 1] / A[n - 1, n - 1];
        for (int i = n - 2; i >= 0; i--)
        {
            float sum = 0;
            for (int j = i + 1; j < n; j++)
            {
                sum += A[i, j] * x[j];
            }
            x[i] = (b[i] - sum) / A[i, i];
        }

        return x;
    }

    private void SaveMatrixToCSV(int[,] matrix, string filename)
    {
        string filePath = Path.Combine(Application.dataPath, filename);
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                string line = "";
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    line += matrix[i, j].ToString();
                    if (j < matrix.GetLength(1) - 1)
                    {
                        line += ",";
                    }
                }
                writer.WriteLine(line);
            }
        }
        Debug.Log($"Saved {filename} to {filePath}");
    }
} 
