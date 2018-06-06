import errorsService from '@/services/errorsService'

export default {
	name:'promisesService',
	onApi:function(promise,onSuccess,onError,errorString){
		if(onSuccess){
			promise = promise.then(function(result){
				if(Array.isArray(onSuccess)){
					for(var i=0;i<onSuccess.length;i++){
						if(onSuccess[i]){
							onSuccess[i](result.data);
						}
					}	
				}else{
					onSuccess(result.data);
				}
			});
		}
		if(onError){
			promise = promise.catch(function(error){
				if(Array.isArray(onError)){
					for(var i=0;i<onError.length;i++){
						if(onError[i]){
							onError[i](error);
						}
					}	
				}else{
					onError(error);
				}
			});
		}else{
			promise = promise.catch(function(error){
				console.log(error);
				errorsService.onError(errorString);
			});
			
		}
	},
	build:function(promise){
		return {
			thens:[],
			catches:[],
			then:function(thenFunction,sync){
				if(sync===undefinded){
					sync=true;
				}
				this.thens.push({
					func:thenFunction,
					sync:sync});
				return this;
			},
			catch:function(catchFunction){
				this.catches.push(catchFunction)
				return this;
			},
			run:function(){
				return promise.then(function(data){
					for(var i=0;i< this.thens.length;i++){
						this.thens[i](data);
					}
				}).catch(function(error){
					for(var i=0;i< this.catches.length;i++){
						this.catches[i](error);
					}
				});
				
			}
			
		}
	},
	fakePromise:function(toReturn){
		return {
			then:function(toDo){
				toDo(toReturn);
				return {
					catch:function(xxx){}
				}
			},
			catch:function(err){
				err(toReturn);
			}
		}
	},
	fakePromiseError:function(toReturn){
		
		return {
			then:function(err){
				return {
					catch:function(funcerr){
						funcerr(toReturn);
					}
				}
			}
		}
	}
}