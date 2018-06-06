import axios from 'axios'
import settings from '@/settings.js'
//import fileService_file from '@/mock/fileService_file' 

export default {
	name:'treeService',
	toPo:function(data){
		return {
			id:data.Id,
			label:data.Name,
			url:data.Url,
			userName:data.UserName,
			notes:data.Notes,
			isFolder:data.IsFolder
		}
	},
	updatePassword:function(id,newPass,newRepeat){
		return axios.post(
				settings.api('/tree/'+id+'/password'),{
					New:newPass,
					NewRepeat:newRepeat
				});
	},
	loadPassword:function(id){
		return axios.get(
				settings.api('/tree/'+id+'/password'));
	},
	doSearch:function(id){
		return axios.get(
				settings.api('/tree/search/'+id));
	},
	loadNodeById:function(id){
		if(id){
			return axios.get(
				settings.api('/tree/'+id));
		}else{
			return axios.get(
				settings.api('/tree/rootitem'));
		}
	},
	loadNodeFullDataById:function(id){
		if(id){
			return axios.get(
				settings.api('/tree/'+id+'/fullData'));
		}else{
			return axios.get(
				settings.api('/tree/rootitem/fullData'));
		}
	},
	loadChildrenById:function(id){
		if(settings.isDebug){
			var root = getTreeById(fakeTreeData,id);
			if(root==null)root =fakeTreeData;
			var children = [];
			
			for(var i=0;i<root.Children.length;i++){
				children.push(this.toPo(root.Children[i]));
			}
			
			return settings.fakePromise(children);
		}
		return axios.get(
			settings.api('/tree/'+id+'/children'));
	},
	deleteItem:function(id){
		return axios.delete(
				settings.api('/tree/'+id));
	},
	moveItem:function(whatId,toId){
		return axios.put(
				settings.api('/tree/'+whatId+"/move/"+toId));
	},
	saveItem:function(newData){
		if(newData.Id==undefined||newData.Id==''||newData.Id==null){
			return axios.post(
				settings.api('/tree'), newData);
		}else{
			return axios.put(
				settings.api('/tree'), newData);
		}
		
	}
}