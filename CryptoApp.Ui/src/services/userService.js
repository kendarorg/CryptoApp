import axios from 'axios'
import settings from '@/settings.js'

export default {
	name:'userService',
	getById:function(model){
		return axios.get(
			settings.api('/users/'+model));
	},
	getAll:function(model){
		return axios.get(
			settings.api('/users'));
	},
	save:function(model){
		console.log(model);
		if(model.Id && model.Id!="" && model.Id!=null){
			return axios.put(
				settings.api('/users/'+model.Id),model);
		}else{
			return axios.post(
				settings.api('/users'),model);				
		}
	},
	savePassword:function(model){
		console.log(model);
		console.log(model.Id);
		return axios.put(
			settings.api('/users/'+model.Id+"/password"),model);
	},
	delete:function(model){
		return axios.delete(
			settings.api('/users/'+model));
	},
}
