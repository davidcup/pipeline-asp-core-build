
def version = "1.0.0"
def slackChannel = "alerts"


pipeline {

    options { 
          disableConcurrentBuilds() 
	      buildDiscarder(logRotator(numToKeepStr: '5', artifactNumToKeepStr: '5'))
	}

    agent {
		node {
		  label 'dotnetcore'
		  customWorkspace '/home/david/jenkins/workspace/github-davidcup'
		}
    }	
  
    stages {
	    stage('Restore') {		
             steps {		
		     	sh(script: 'dotnet restore AspNetCoreApiDemo.sln', returnStdout: true)	
			 }
	    }
		  
		stage('Sonar Begin') {
			environment {
				sonarqubeScannerHome = tool 'SonarQubeScanner'
			}
			steps {
        withSonarQubeEnv('sonarqube') {
            sh "${scannerHome}/bin/sonar-scanner"
        }
        timeout(time: 10, unit: 'MINUTES') {
            waitForQualityGate abortPipeline: true
        }
    }
        }  
		  
        stage('Build') {
            steps {
               sh "dotnet build AspNetCoreApiDemo.sln -c Release"
            }
        }
		
		
    }
  
}