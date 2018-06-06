import fileService from '@/services/fileService'
import proms from '@/services/promisesService'
	
export default {
	name:'fileLoaderController',
	uploadFile:function(fileRef,fileLabel,encryptionKey,onSuccess,onError){
		proms.onApi(
			fileService.uploadFile(fileRef,fileLabel,encryptionKey),
			onSuccess,
			onError,
			'Upload Error');
	},
	createFile:function(model,onSuccess,onError){
		proms.onApi(
			fileService.createFile(model),
			onSuccess,
			onError,
			'Create Error');
	},
	loadFile:function(model,onSuccess,onError){
		proms.onApi(
			fileService.loadFile(model),
			onSuccess,
			onError,
			'Load files error');
	},
	deleteFile:function(model,onSuccess,onError){
		proms.onApi(
			fileService.deleteFile(model),
			onSuccess,
			onError,
			'Load files error');
	},
	loadFileList:function(onSuccess,onError){
		proms.onApi(
			fileService.loadAvailableFiles(),
			onSuccess,
			onError,
			'Error loadin file list');
	}
}