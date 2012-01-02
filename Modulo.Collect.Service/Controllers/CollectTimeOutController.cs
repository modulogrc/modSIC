/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */


namespace Modulo.Collect.Service.Controllers
{
    public class CollectTimeOutController
    {
        private const int DEFAULT_COLLECT_EXECUTION_TIMEOUT = 3;
        private const int DEFAULT_ATTEMPTS_EVALUATE_VARIABLES = 5;

        /// <summary>
        /// Get the execution timeout value configured from configuration file.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfCollectExecution()
        {
            string timeOutValue = System.Configuration.ConfigurationManager.AppSettings["CollectExecutionTimeOut"]; 
            if (string.IsNullOrEmpty(timeOutValue))
                return DEFAULT_COLLECT_EXECUTION_TIMEOUT;
            else
                return int.Parse(timeOutValue);
        }

        /// <summary>
        /// This methods verifies if the number of executions over the limit defined.
        /// </summary>
        /// <param name="numberOfExecution">the number of executions already made</param>
        /// <returns></returns>
        public bool IsExceededTheMaxOfExecutionsDefined(int numberOfExecution)
        {
            return numberOfExecution > this.GetNumberOfCollectExecution();
        }

        /// <summary>
        /// This methods verifies if the number of attempts of evaluate the variables over the limit defined.
        /// </summary>
        /// <param name="Attempts">the number of attempts already made</param>
        /// <returns></returns>
        public bool IsExceededTheMaxAttemptsOfEvaluateVariables(int attempts)
        {
            return attempts > DEFAULT_ATTEMPTS_EVALUATE_VARIABLES;
        }

    }
}
