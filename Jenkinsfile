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
        stage('build') {
            steps {
               sh "echo 'hello from docker inside docker'"
            }
        }
    }
  
}