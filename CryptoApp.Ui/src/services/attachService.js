import axios from 'axios'
import settings from '@/settings.js'
//import fileService_file from '@/mock/fileService_file' 

export default {
	name:'attachService',
	uploadFile:function(file,fileId){
		let formData = new FormData();
        formData.append('file',file);
        //console.log(file);
		//In PHP it’d be: $_FILES['file']
		return axios.post( settings.api('/attach/'+fileId),
		  formData,
		  {
		    headers: {
		        'Content-Type': 'multipart/form-data'
		    }
		  }
		);
	}
}
