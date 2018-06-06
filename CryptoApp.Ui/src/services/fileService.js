import axios from 'axios'
import settings from '@/settings.js'
//import fileService_file from '@/mock/fileService_file' 

export default {
	name:'fileService',
	createFile:function(model,encryptionKey){
		
		//In PHP it’d be: $_FILES['file']
		return axios.post( 
			settings.api('/files/create/new'),model);
	},
	uploadFile:function(file,fileName,encryptionKey){
		let formData = new FormData();
        formData.append('file',file);
        formData.append('encryptionKey',encryptionKey);
		//In PHP it’d be: $_FILES['file']
		return axios.post( settings.api('/files/')+fileName,
		  formData,
		  {
		    headers: {
		        'Content-Type': 'multipart/form-data'
		    }
		  }
		);
	},
	loadAvailableFiles:function(){
		return axios.get(
			settings.api('/files'));
	},
	loadFile:function(model){
		return axios.post(
			settings.api('/files/load'), 
			{
				FileName: model.FileName,
				FileKey: model.FileKey
		});
	},
	deleteFile:function(model){
		return axios.delete(
			settings.api('/files/'+model));
	}
}