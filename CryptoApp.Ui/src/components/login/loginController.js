import authService from '@/services/authService'
import proms from '@/services/promisesService'

export default {
	name:'loginController',
	login:function(loginModel,onSuccess,onError){
		proms.onApi(
			authService.login(loginModel),
			onSuccess,
			onError,
			'Login Error');
	},
	logoff:function(onSuccess){
		proms.onApi(
			authService.logoff(),
			onSuccess,
			onSuccess,
			'Login Error');
	}
}