import axios from 'axios'
import settings from '@/settings.js'

export default {
	name:'authService',
	login:function(model){
		return axios.post(
			settings.api('/user/login'),model);
	},
	logoff:function(){
		return axios.get(
			settings.api('/user/logoff'));
	}
}
