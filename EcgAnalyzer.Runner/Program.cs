using System;
using System.Collections.Generic;
using System.Linq;

namespace EcgAnalyzer.Runner
{
    class Program
    {
        static void Main(string[] args)
        {          
            var normalDirs = new string[] {
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu01",
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu11",
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu12",
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu17",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu24"
                                          };
            var arrhythmiaDirs = new string[] {
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu01",
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu11",
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu12",
                                            //@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu17",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu24"
                                          };

            
            var modelParams = new List<WaveformModelParameters>() {
                                                    new WaveformModelParameters {  States = 2, Symbols = 3},                                                  
                                                    new WaveformModelParameters {  States = 2, Symbols = 5},
                                                    new WaveformModelParameters {  States = 3, Symbols = 3},                                                 
                                                    new WaveformModelParameters {  States = 3, Symbols = 5},                                                   
                                                    new WaveformModelParameters {  States = 5, Symbols = 5 },
                                                    new WaveformModelParameters {  States = 5, Symbols = 8 },
                                                    new WaveformModelParameters {  States = 10, Symbols = 10 } };

            var single = new SinglePatientNormalVsArrhythmiaTest(normalDirs, arrhythmiaDirs);

            foreach (var modelParam in modelParams)
            {
                single.Run(modelParam);
            }

         //   new MultiplePatientNormalRhythmTest(normalDirs).Run();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }        
    }    
}
