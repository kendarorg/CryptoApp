import treeService from '@/services/treeService'
import proms from '@/services/promisesService'
import clipboardService from '@/services/clipboardService'
import attachService from '@/services/attachService'

export default {
	name:'treeController',
	loadTreeSuccess:function(result){
		return {
			label:result.Title,
			id:result.Id,
			isFolder:result.IsFolder,
			nodes:[]
		};
	},
	///// NEW
	loadChildrenById:function(id,onSuccess,onError){
		proms.onApi(
				treeService.loadChildrenById(id),
				onSuccess,
				onError,
				'Error loading node '+id);
	},
	loadNodeById:function(id,action,onSuccess,onError){

		if(action=="show"){
			proms.onApi(
				treeService.loadNodeById(id?id:'rootitem'),
				onSuccess,
				onError,
				'Error loading node '+id);
		}else{
			if(onSuccess)onSuccess({
				Id:'',
				Title:''
			});
		}
	},
	loadNodeFullDataById:function(action,id,onSuccess,onError){
		if(action=="show"){
			proms.onApi(
				treeService.loadNodeFullDataById(id?id:'rootitem'),
				onSuccess,
				onError,
				'Error loading node detail for '+id);
		}else{
			if(onSuccess)onSuccess({
				Id:'',
				Title:'',
				Url:'',
				Notes:'',
				UserName:'',
				ExpireTime:''
			});
		}
	},
	saveItem:function(model,onSuccess,onError){
		
		proms.onApi(treeService.saveItem(model),
			onSuccess,
			onError,
			'Error saving item!');
	},
	deleteItem:function(id,onSuccess,onError){
		
		proms.onApi(treeService.deleteItem(id),
			onSuccess,
			onError,
			'Error deleting item!');
	},
	moveItem:function(toMoveId,targetId,onSuccess,onError){
		proms.onApi(treeService.moveItem(toMoveId,targetId),
			onSuccess,
			onError,
			'Error Moving item!');
	},
	loadPassword:function(id,onSuccess,onError){
		proms.onApi(treeService.loadPassword(id),
			onSuccess,
			onError,
			'Error Loading password!');
	},
	updatePassword:function(id,newPass,newRepeat,onSuccess,onError){
		proms.onApi(treeService.updatePassword(id,newPass,newRepeat),
			onSuccess,
			onError,
			'Error Changing password!');
	},
	doSearch:function(searchstr,onSuccess,onError){
		proms.onApi(treeService.doSearch(searchstr),
			onSuccess,
			onError,
			'Error Searching!');
	},
	uploadAttachment:function(file,id,onSuccess,onError){
		proms.onApi(attachService.uploadFile(file,id),
			onSuccess,
			onError,
			'Error Uploading!');
	}
}