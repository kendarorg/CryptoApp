import axios from 'axios'
import settings from '@/settings.js'
import router from '@/router/index'


export default {
	name:'errorsService',
	onError:function(message){
		router.push({ name: 'errors', params: {message:message} });
	}
}