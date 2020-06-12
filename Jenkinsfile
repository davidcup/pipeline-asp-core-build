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
			sh(script: 'dotnet restore AspNetCoreApiDemo.sln', returnStdout: true);			
		  }
		  
        stage('Build') {
            steps {
               sh "dotnet build AspNetCoreApiDemo.sln -c Release"
            }
        }
    }
  
}