export default {
	name:'settings',
	apiRoot:'/api',
	isDebug:false,
	api:function(val){
		return this.apiRoot+val;
	}
}