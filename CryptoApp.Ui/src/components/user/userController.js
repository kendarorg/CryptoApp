import userService from '@/services/userService'
import proms from '@/services/promisesService'

export default {
	name:'userController',
	getById:function(id,onSuccess,onError){
		proms.onApi(
			userService.getById(id),
			onSuccess,
			onError,
			'Get User');
	},
	delete:function(id,onSuccess,onError){
		proms.onApi(
			userService.delete(id),
			onSuccess,
			onError,
			'Get User');
	},
	getAll:function(onSuccess,onError){
		proms.onApi(
			userService.getAll(),
			onSuccess,
			onError,
			'Get User');
	},
	save:function(model,onSuccess,onError){
		proms.onApi(
			userService.save(model),
			onSuccess,
			onError,
			'Get User');
	},
	savePassword:function(model,onSuccess,onError){
		proms.onApi(
			userService.savePassword(model),
			onSuccess,
			onError,
			'Get User');
	}
}