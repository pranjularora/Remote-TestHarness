///////////////////////////////////////////////////////////////////////////////////
// ITest.cs - Interface for Test Drivers                                         //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * This Module is responsible for declaring the function which has to be implemented by all the classes 
 * implementing it
 * 
 * Functions:
 * ===================
 * bool test()
 * These will be defined by the classes which will implement it.
 * 
 *
 * Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 No changes in this module
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 *  
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    // Interface used by Test Drivers
    public interface ITest
    {
        bool test();
    }
}
