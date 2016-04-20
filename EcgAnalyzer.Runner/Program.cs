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
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu01",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu11",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu12",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu17",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu24"
                                          };
            var arrhythmiaDirs = new string[] {
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu01",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu11",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu12",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu17",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu24"
                                          };

            new SinglePatientNormalVsArrhythmiaTest(normalDirs, arrhythmiaDirs).Run();
            new MultiplePatientNormalRhythmTest(normalDirs).Run();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }        
    }    
}
